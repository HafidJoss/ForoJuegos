using NuevoForo.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using NuevoForo.Infrastructure.Services;
using NuevoForo.Infrastructure.Services.Import;
using NuevoForo.Infrastructure.Data.Seeders;
using NuevoForo.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configurar FormOptions para descargas de archivos ilimitadas/grandes (UGC)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue; // Cargas ilimitadas
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar servicios de importación
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<JuegoSeeder>();

// Configurar HttpClient para descargas
builder.Services.AddHttpClient<IImportService, ImportService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Add("User-Agent", "NuevoForo/1.0");
});

builder.Services.AddMemoryCache();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
        name: "db",
        tags: new[] { "db" });

builder.Services.AddRateLimiter(options =>
{
    var global = builder.Configuration.GetSection("RateLimiting:Global");
    var auth = builder.Configuration.GetSection("RateLimiting:Auth");

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = global.GetValue<int>("PermitLimit"),
                Window = TimeSpan.FromSeconds(global.GetValue<int>("WindowSeconds")),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = auth.GetValue<int>("PermitLimit"),
                Window = TimeSpan.FromSeconds(auth.GetValue<int>("WindowSeconds")),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Rate limit exceeded.");
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.SetIsOriginAllowed(origin =>
            Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["SigningKey"] ?? string.Empty;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ============ CONFIGURACIÓN DE ARCHIVOS ESTÁTICOS PARA UGC ============
// Obtener la ruta de contenido (wwwroot o carpeta raíz del app)
// Si WebRootPath es nulo, usar ContentRootPath + "wwwroot"
string webRootPath = app.Environment.WebRootPath ?? 
    Path.Combine(app.Environment.ContentRootPath, "wwwroot");

// Asegurar que la carpeta wwwroot existe
if (!Directory.Exists(webRootPath))
{
    Directory.CreateDirectory(webRootPath);
}

// Crear carpetas de uploads, ugc y fotos si no existen
var uploadsPath = Path.Combine(webRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    app.Logger.LogInformation("Carpeta de uploads creada en: {UploadsPath}", uploadsPath);
}

var ugcPath = Path.Combine(uploadsPath, "ugc");
if (!Directory.Exists(ugcPath))
{
    Directory.CreateDirectory(ugcPath);
    app.Logger.LogInformation("Carpeta de UGC creada en: {UgcPath}", ugcPath);
}

var fotosPath = Path.Combine(uploadsPath, "fotos");
if (!Directory.Exists(fotosPath))
{
    Directory.CreateDirectory(fotosPath);
    app.Logger.LogInformation("Carpeta de fotos creada en: {FotosPath}", fotosPath);
}

// Servir archivos estáticos desde wwwroot (incluye /uploads, /uploads/ugc y /uploads/fotos)
app.UseStaticFiles();

// Agregar encabezados de seguridad para descargas
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/uploads"))
    {
        // Permitir descarga de archivos con Content-Disposition
        context.Response.Headers.AccessControlAllowOrigin = "*";
        context.Response.Headers.CacheControl = "public, max-age=3600";
    }
    await next();
});
// ====================================================================== 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.UseRateLimiter();

app.Use(async (context, next) =>
{
    var requestId = context.TraceIdentifier;
    var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    using (app.Logger.BeginScope(new Dictionary<string, object?>
    {
        ["RequestId"] = requestId,
        ["UserId"] = userId
    }))
    {
        await next();
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK
    }
});
app.MapHealthChecks("/health/db", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db") || check.Name == "db"
});

app.MapControllers();
app.MapGroup("/auth").RequireRateLimiting("auth");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await RoleSeeder.SeedAsync(roleManager);

    // ============ SEEDING DE JUEGOS DESDE STEAM ============
    try
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Verificar si hay juegos en la BD
        var juegoCount = dbContext.Juegos.Count();
        if (juegoCount == 0)
        {
            logger.LogInformation("No se encontraron juegos en la BD. Iniciando seeding desde Steam dataset...");

            var juegoSeeder = scope.ServiceProvider.GetRequiredService<JuegoSeeder>();

            // Obtener la ruta de datos (carpeta data en raíz del proyecto o ContentRootPath)
            var dataDirectory = Path.Combine(app.Environment.ContentRootPath, "data");

            var result = await juegoSeeder.SeedJuegosFromSteamAsync(dataDirectory);

            if (result.IsSuccess)
            {
                logger.LogInformation("✅ Seeding de juegos completado exitosamente: {Summary}", result.Summary);
            }
            else
            {
                logger.LogWarning("⚠️ Seeding de juegos completado con advertencias: {Summary}", result.Summary);
                foreach (var error in result.Errors)
                {
                    logger.LogWarning("  Error: {Error}", error);
                }
            }
        }
        else
        {
            logger.LogInformation("✅ BD ya contiene {Count} juegos. Seeding omitido.", juegoCount);
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error durante el seeding de juegos");
    }
    // =====================================================
}

app.Run();
