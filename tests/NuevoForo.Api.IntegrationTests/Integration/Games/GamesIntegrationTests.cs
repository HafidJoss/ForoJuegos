using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Api.IntegrationTests.Integration.Games;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Juegos.
/// </summary>
[TestClass]
public class GamesIntegrationTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;
    private AppDbContext _dbContext = null!;

    /// <summary>
    /// Se ejecuta antes de cada prueba.
    /// </summary>
    public async Task InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.InitializeAsync();
        _dbContext = _fixture.DbContext;
    }

    /// <summary>
    /// Se ejecuta después de cada prueba.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [TestMethod]
    [Description("Verifica que se puede crear un juego en la base de datos")]
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

        Assert.IsNotNull(gameSaved);
        Assert.AreEqual("The Witcher 3", gameSaved.Nombre);
        Assert.AreEqual("PC", gameSaved.Plataforma);
        Assert.IsTrue(gameSaved.Activo);
    }

    [TestMethod]
    [Description("Verifica que se puede actualizar los datos de un juego")]
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

        Assert.AreEqual("Game Actualizado", gameVerified.Nombre);
        Assert.AreEqual("Nueva descripción", gameVerified.Descripcion);
    }

    [TestMethod]
    [Description("Verifica que se puede recuperar un juego con sus reseñas relacionadas")]
    public async Task GetGame_WithReviews_ShouldLoadRelatedEntities()
    {
        // Arrange
        var game = await _fixture.GetOrCreateTestGameAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user.Id)
            .WithTitulo("Reseña 1")
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user.Id)
            .WithTitulo("Reseña 2")
            .Build();

        _dbContext.Resenas.AddRange(review1, review2);
        await _dbContext.SaveChangesAsync();

        // Act
        var gameWithReviews = await _dbContext.Juegos
            .Include(g => g.Resenas)
            .FirstOrDefaultAsync(g => g.Id == game.Id);

        // Assert
        Assert.IsNotNull(gameWithReviews);
        Assert.AreEqual(2, gameWithReviews.Resenas?.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede buscar juegos por nombre")]
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
        Assert.AreEqual(1, darkSoulsGames.Count);
        Assert.AreEqual("Dark Souls 3", darkSoulsGames[0].Nombre);
    }

    [TestMethod]
    [Description("Verifica que se puede filtrar juegos por plataforma")]
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
            .Where(g => g.Plataforma == "PC" && g.Activo)
            .ToListAsync();

        // Assert
        Assert.AreEqual(2, pcGames.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede obtener el promedio de calificaciones de un juego")]
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
            .WithCalificacion(5)
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user2.Id)
            .WithCalificacion(4)
            .Build();

        var review3 = TestDataBuilder.CreateReview()
            .WithJuegoId(game.Id)
            .WithUsuarioId(user3.Id)
            .WithCalificacion(3)
            .Build();

        _dbContext.Resenas.AddRange(review1, review2, review3);
        await _dbContext.SaveChangesAsync();

        // Act
        var averageRating = await _dbContext.Resenas
            .Where(r => r.JuegoId == game.Id && r.Activo)
            .AverageAsync(r => r.Calificacion);

        // Assert
        Assert.AreEqual(4.0, averageRating, 0.01);
    }

    [TestMethod]
    [Description("Verifica que se puede desactivar un juego (soft delete)")]
    public async Task DeactivateGame_ShouldMarkAsInactive()
    {
        // Arrange
        var game = await _fixture.GetOrCreateTestGameAsync();

        // Act
        var gameToDeactivate = await _dbContext.Juegos
            .FirstAsync(g => g.Id == game.Id);

        gameToDeactivate.Activo = false;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deactivatedGame = await _dbContext.Juegos
            .FirstAsync(g => g.Id == game.Id);

        Assert.IsFalse(deactivatedGame.Activo);
    }

    [TestMethod]
    [Description("Verifica que se pueden listar solo juegos activos")]
    public async Task ListActiveGames_ShouldExcludeInactiveGames()
    {
        // Arrange
        var activeGame = await _fixture.GetOrCreateTestGameAsync();

        var inactiveGame = TestDataBuilder.CreateGame()
            .WithActivo(false)
            .Build();

        _dbContext.Juegos.Add(inactiveGame);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeGames = await _dbContext.Juegos
            .Where(g => g.Activo)
            .ToListAsync();

        // Assert
        Assert.IsTrue(activeGames.All(g => g.Activo));
    }
}
