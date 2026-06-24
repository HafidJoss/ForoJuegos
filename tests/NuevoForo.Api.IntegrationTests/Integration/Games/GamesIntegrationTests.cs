using Microsoft.EntityFrameworkCore;
using Xunit;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.IntegrationTests.Integration.Games;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Juegos.
/// </summary>
public class GamesIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private AppDbContext _dbContext = null!;

    public GamesIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.DbContext;
        await _fixture.ClearDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    [Trait("Description", "Verifica que se puede crear un juego en la base de datos")]
    public async Task CreateGame_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var game = TestDataBuilder.CreateGame()
            .WithNombre("The Witcher 3")
            .WithDescripcion("Juego de rol de aventura")
            .WithPlataforma("PC")
            .Build();

        // Act
        _dbContext.Juegos.Add(game);
        await _dbContext.SaveChangesAsync();

        // Assert
        var gameSaved = await _dbContext.Juegos
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        Assert.NotNull(gameSaved);
        Assert.Equal("The Witcher 3", gameSaved.Nombre);
        Assert.Equal("PC", gameSaved.Plataforma);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede actualizar los datos de un juego")]
    public async Task UpdateGame_WithModifiedData_ShouldPersistChanges()
    {
        // Arrange
        var game = await _fixture.GetOrCreateTestGameAsync("Game Original");

        // Act
        var gameToUpdate = await _dbContext.Juegos
            .FirstAsync(g => g.Id == game.Id);

        gameToUpdate.Nombre = "Game Actualizado";
        gameToUpdate.Descripcion = "Nueva descripción";
        await _dbContext.SaveChangesAsync();

        // Assert
        var gameVerified = await _dbContext.Juegos
            .FirstAsync(g => g.Id == game.Id);

        Assert.Equal("Game Actualizado", gameVerified.Nombre);
        Assert.Equal("Nueva descripción", gameVerified.Descripcion);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar un juego con sus reseñas relacionadas")]
    public async Task GetGame_WithReviews_ShouldLoadRelatedEntities()
    {
        // Arrange
        var game = await _fixture.GetOrCreateTestGameAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user1.Id)
            .WithTexto("Reseña 1")
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user2.Id)
            .WithTexto("Reseña 2")
            .Build();

        _dbContext.Resenas.AddRange(review1, review2);
        await _dbContext.SaveChangesAsync();

        // Act
        var gameWithReviews = await _dbContext.Juegos
            .Include(g => g.Resenas)
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        // Assert
        Assert.NotNull(gameWithReviews);
        Assert.Equal(2, gameWithReviews.Resenas?.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede buscar juegos por nombre")]
    public async Task SearchGames_ByNombre_ShouldReturnMatches()
    {
        // Arrange
        var game1 = TestDataBuilder.CreateGame()
            .WithNombre("Elden Ring")
            .Build();

        var game2 = TestDataBuilder.CreateGame()
            .WithNombre("Dark Souls 3")
            .Build();

        var game3 = TestDataBuilder.CreateGame()
            .WithNombre("Cyberpunk 2077")
            .Build();

        _dbContext.Juegos.AddRange(game1, game2, game3);
        await _dbContext.SaveChangesAsync();

        // Act
        var darkSoulsGames = await _dbContext.Juegos
            .Where(g => g.Nombre.Contains("Dark"))
            .ToListAsync();

        // Assert
        Assert.Equal(1, darkSoulsGames.Count);
        Assert.Equal("Dark Souls 3", darkSoulsGames[0].Nombre);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede filtrar juegos por plataforma")]
    public async Task FilterGames_ByPlataforma_ShouldReturnMatches()
    {
        // Arrange
        var pcGame1 = TestDataBuilder.CreateGame()
            .WithNombre("Game PC 1")
            .WithPlataforma("PC")
            .Build();

        var pcGame2 = TestDataBuilder.CreateGame()
            .WithNombre("Game PC 2")
            .WithPlataforma("PC")
            .Build();

        var playStationGame = TestDataBuilder.CreateGame()
            .WithNombre("Game PS5")
            .WithPlataforma("PlayStation")
            .Build();

        _dbContext.Juegos.AddRange(pcGame1, pcGame2, playStationGame);
        await _dbContext.SaveChangesAsync();

        // Act
        var pcGames = await _dbContext.Juegos
            .Where(g => g.Plataforma == "PC")
            .ToListAsync();

        // Assert
        Assert.Equal(2, pcGames.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede obtener el promedio de calificaciones de un juego")]
    public async Task GetGameAverageRating_ShouldCalculateCorrectly()
    {
        // Arrange
        var game = await _fixture.GetOrCreateTestGameAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();
        var user3 = await _fixture.GetOrCreateTestUserAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user1.Id)
            .WithRating(5)
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user2.Id)
            .WithRating(4)
            .Build();

        var review3 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user3.Id)
            .WithRating(3)
            .Build();

        _dbContext.Resenas.AddRange(review1, review2, review3);
        await _dbContext.SaveChangesAsync();

        // Act
        var averageRating = await _dbContext.Resenas
            .Where(r => r.JuegoId == game.Id && r.Estado == EstadoResena.Activa)
            .AverageAsync(r => r.Rating);

        // Assert
        Assert.Equal(4.0, averageRating, 0.01);
    }
}
