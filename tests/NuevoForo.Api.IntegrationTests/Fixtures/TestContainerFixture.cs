using Microsoft.EntityFrameworkCore;
using NuevoForo.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace NuevoForo.Api.IntegrationTests.Fixtures;

/// <summary>
/// Fixture que maneja el ciclo de vida del contenedor PostgreSQL para pruebas de integración.
/// Implementa IAsyncLifetime para inicializar y limpiar recursos de forma automática.
/// </summary>
public class TestContainerFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private DbContextOptions<AppDbContext> _dbContextOptions = null!;

    /// <summary>
    /// Obtiene las opciones configuradas para el DbContext.
    /// </summary>
    public DbContextOptions<AppDbContext> DbContextOptions => _dbContextOptions;

    /// <summary>
    /// Crea una nueva instancia de AppDbContext conectada al contenedor.
    /// </summary>
    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_dbContextOptions);
    }

    /// <summary>
    /// Se ejecuta antes de cada prueba. Inicia el contenedor PostgreSQL y aplica migraciones.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Construir y configurar el contenedor PostgreSQL
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            // Usar PostgreSQL 15 (compatible con la BD de producción)

            .WithDatabase("nuevoforo_test")
            // Nombre de la BD de prueba

            .WithUsername("postgres")
            // Usuario por defecto

            .WithPassword("testpassword123")
            // Contraseña para pruebas (no usada en producción)

            .WithCleanUp(true)
            // Eliminar contenedor automáticamente después

            .Build();

        // Iniciar el contenedor
        await _container.StartAsync();

        // Obtener la cadena de conexión generada por Testcontainers
        var connectionString = _container.GetConnectionString();

        // Configurar DbContextOptions para usar la conexión temporal
        _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        // Crear la BD y aplicar todas las migraciones
        using (var dbContext = new AppDbContext(_dbContextOptions))
        {
            // Crear las tablas usando migraciones
            await dbContext.Database.MigrateAsync();
        }
    }

    /// <summary>
    /// Se ejecuta después de cada prueba. Detiene y elimina el contenedor PostgreSQL.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            // Detener el contenedor
            await _container.StopAsync();

            // Eliminar el contenedor
            await _container.DisposeAsync();
        }
    }
}
