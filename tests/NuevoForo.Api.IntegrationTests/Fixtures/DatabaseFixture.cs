using Microsoft.EntityFrameworkCore;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Api.IntegrationTests.Fixtures;

/// <summary>
/// Fixture que proporciona helpers para trabajar con la BD durante las pruebas.
/// Gestiona la creación de DbContext, limpieza de datos y seeding.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly TestContainerFixture _testContainerFixture;
    private AppDbContext? _dbContext;

    public DatabaseFixture()
    {
        _testContainerFixture = new TestContainerFixture();
    }

    /// <summary>
    /// Obtiene la instancia actual del DbContext.
    /// </summary>
    public AppDbContext DbContext
    {
        get
        {
            if (_dbContext == null)
            {
                throw new InvalidOperationException("DbContext no ha sido inicializado. Asegúrate de que InitializeAsync ha sido llamado.");
            }
            return _dbContext;
        }
    }

    /// <summary>
    /// Inicializa el contenedor y el DbContext.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _testContainerFixture.InitializeAsync();
        _dbContext = _testContainerFixture.CreateDbContext();
    }

    /// <summary>
    /// Limpia el DbContext y detiene el contenedor.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
        await _testContainerFixture.DisposeAsync();
    }

    /// <summary>
    /// Limpia todos los datos de la BD (usa DELETE para cada tabla).
    /// </summary>
    public async Task ClearDatabaseAsync()
    {
        // Orden importante: eliminar tablas con dependencias primero (FK)
        DbContext.LikesUgc.RemoveRange(DbContext.LikesUgc);
        DbContext.ContenidosUgc.RemoveRange(DbContext.ContenidosUgc);
        DbContext.LikesResena.RemoveRange(DbContext.LikesResena);
        DbContext.Comentarios.RemoveRange(DbContext.Comentarios);
        DbContext.Resenas.RemoveRange(DbContext.Resenas);
        DbContext.Juegos.RemoveRange(DbContext.Juegos);
        DbContext.Reportes.RemoveRange(DbContext.Reportes);
        DbContext.Notificaciones.RemoveRange(DbContext.Notificaciones);
        DbContext.Donaciones.RemoveRange(DbContext.Donaciones);
        DbContext.Users.RemoveRange(DbContext.Users);

        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene o crea un usuario de prueba.
    /// </summary>
    public async Task<Usuario> GetOrCreateTestUserAsync(
        string? id = null,
        string? userName = null,
        string? email = null)
    {
        var userId = Guid.Parse(id ?? Guid.NewGuid().ToString());
        var user = new Usuario
        {
            Id = userId,
            UserName = userName ?? $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}",
            NormalizedUserName = (userName ?? $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}").ToUpper(),
            Email = email ?? $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com",
            NormalizedEmail = (email ?? $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com").ToUpper(),
            Nombre = "Test",
            Apellido = "User",
            EmailConfirmed = true,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        DbContext.Usuarios.Add(user);
        await DbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Obtiene o crea un juego de prueba.
    /// </summary>
    public async Task<Juego> GetOrCreateTestGameAsync(string? nombre = null)
    {
        var game = new Juego
        {
            Id = Guid.NewGuid(),
            Nombre = nombre ?? $"Test Game {Guid.NewGuid().ToString().Substring(0, 8)}",
            Descripcion = "Juego de prueba",
            Plataforma = "PC",
            FechaLanzamiento = DateTime.UtcNow.AddDays(-30),
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        DbContext.Juegos.Add(game);
        await DbContext.SaveChangesAsync();
        return game;
    }

    /// <summary>
    /// Obtiene o crea una reseña de prueba.
    /// </summary>
    public async Task<Resena> GetOrCreateTestReviewAsync(
        Usuario? user = null,
        Juego? game = null,
        string? titulo = null,
        int calificacion = 5)
    {
        user ??= await GetOrCreateTestUserAsync();
        game ??= await GetOrCreateTestGameAsync();

        var review = new Resena
        {
            Id = Guid.NewGuid(),
            UsuarioId = user.Id,
            JuegoId = game.Id,
            Titulo = titulo ?? "Test Review",
            Contenido = "Esta es una reseña de prueba",
            Calificacion = calificacion,
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        DbContext.Resenas.Add(review);
        await DbContext.SaveChangesAsync();
        return review;
    }

    /// <summary>
    /// Obtiene o crea un comentario de prueba.
    /// </summary>
    public async Task<Comentario> GetOrCreateTestCommentAsync(
        Resena? review = null,
        Usuario? user = null,
        string? contenido = null)
    {
        review ??= await GetOrCreateTestReviewAsync();
        user ??= await GetOrCreateTestUserAsync();

        var comment = new Comentario
        {
            Id = Guid.NewGuid(),
            ResenaId = review.Id,
            UsuarioId = user.Id,
            Contenido = contenido ?? "Test comment",
            FechaCreacion = DateTime.UtcNow,
            Activo = true
        };

        DbContext.Comentarios.Add(comment);
        await DbContext.SaveChangesAsync();
        return comment;
    }

    /// <summary>
    /// Obtiene o crea un like en una reseña.
    /// </summary>
    public async Task<LikeResena> GetOrCreateTestLikeAsync(
        Resena? review = null,
        Usuario? user = null)
    {
        review ??= await GetOrCreateTestReviewAsync();
        user ??= await GetOrCreateTestUserAsync();

        var like = new LikeResena
        {
            Id = Guid.NewGuid(),
            ResenaId = review.Id,
            UsuarioId = user.Id,
            FechaCreacion = DateTime.UtcNow
        };

        DbContext.LikesResena.Add(like);
        await DbContext.SaveChangesAsync();
        return like;
    }
}
