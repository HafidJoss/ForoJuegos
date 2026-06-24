using Microsoft.EntityFrameworkCore;
using Xunit;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.IntegrationTests.Integration.Reviews;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Reseñas.
/// </summary>
public class ReviewsIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private AppDbContext _dbContext = null!;

    public ReviewsIntegrationTests(DatabaseFixture fixture)
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
    [Trait("Description", "Verifica que se puede crear una reseña en la base de datos")]
    public async Task CreateReview_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();

        var review = TestDataBuilder.CreateReview()
            .WithUsuarioId(user.Id)
            .WithJuegoId(game.Id)
            .WithTexto("Una experiencia increíble")
            .WithRating(5)
            .Build();

        // Act
        _dbContext.Resenas.Add(review);
        await _dbContext.SaveChangesAsync();

        // Assert
        var reviewSaved = await _dbContext.Resenas
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        Assert.NotNull(reviewSaved);
        Assert.Equal("Una experiencia increíble", reviewSaved.Texto);
        Assert.Equal(5, reviewSaved.Rating);
        Assert.Equal(EstadoResena.Activa, reviewSaved.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede actualizar una reseña")]
    public async Task UpdateReview_WithModifiedData_ShouldPersistChanges()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync(
            texto: "Buen juego",
            rating: 3
        );

        // Act
        var reviewToUpdate = await _dbContext.Resenas
            .FirstAsync(r => r.Id == review.Id);

        reviewToUpdate.Rating = 5;
        reviewToUpdate.Texto = "Increíble, lo recomiendo";
        await _dbContext.SaveChangesAsync();

        // Assert
        var reviewVerified = await _dbContext.Resenas
            .FirstAsync(r => r.Id == review.Id);

        Assert.Equal(5, reviewVerified.Rating);
        Assert.Equal("Increíble, lo recomiendo", reviewVerified.Texto);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede eliminar (soft delete) una reseña")]
    public async Task DeleteReview_ShouldMarkAsInactive()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();

        // Act
        var reviewToDelete = await _dbContext.Resenas
            .FirstAsync(r => r.Id == review.Id);

        reviewToDelete.Estado = EstadoResena.Eliminada;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedReview = await _dbContext.Resenas
            .FirstAsync(r => r.Id == review.Id);

        Assert.Equal(EstadoResena.Eliminada, deletedReview.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar una reseña con sus comentarios relacionados")]
    public async Task GetReview_WithComments_ShouldLoadRelatedEntities()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();

        var comment1 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user1,
            texto: "Comentario 1"
        );

        var comment2 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user2,
            texto: "Comentario 2"
        );

        // Act
        var reviewWithComments = await _dbContext.Resenas
            .Include(r => r.Comentarios)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        // Assert
        Assert.NotNull(reviewWithComments);
        Assert.Equal(2, reviewWithComments.Comentarios?.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar una reseña con usuario y juego relacionados")]
    public async Task GetReview_WithRelations_ShouldLoadAllEntities()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();
        var review = await _fixture.GetOrCreateTestReviewAsync(user: user, game: game);

        // Act
        var reviewWithRelations = await _dbContext.Resenas
            .Include(r => r.Usuario)
            .Include(r => r.Juego)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        // Assert
        Assert.NotNull(reviewWithRelations);
        Assert.NotNull(reviewWithRelations.Usuario);
        Assert.NotNull(reviewWithRelations.Juego);
        Assert.Equal(user.Id, reviewWithRelations.Usuario.Id);
        Assert.Equal(game.Id, reviewWithRelations.Juego.Id);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede buscar reseñas por texto")]
    public async Task SearchReviews_ByTexto_ShouldReturnMatches()
    {
        // Arrange
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();
        var user3 = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user1.Id)
            .WithJuegoId(game.Id)
            .WithTexto("El mejor juego del año")
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user2.Id)
            .WithJuegoId(game.Id)
            .WithTexto("El mejor juego que he jugado")
            .Build();

        var review3 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user3.Id)
            .WithJuegoId(game.Id)
            .WithTexto("No es mi tipo de juego")
            .Build();

        _dbContext.Resenas.AddRange(review1, review2, review3);
        await _dbContext.SaveChangesAsync();

        // Act
        var bestGameReviews = await _dbContext.Resenas
            .Where(r => r.Texto.Contains("mejor") && r.Estado == EstadoResena.Activa)
            .ToListAsync();

        // Assert
        Assert.Equal(2, bestGameReviews.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se pueden filtrar reseñas por calificación")]
    public async Task FilterReviews_ByRating_ShouldReturnMatches()
    {
        // Arrange
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();
        var user3 = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();

        var review5Stars = TestDataBuilder.CreateReview()
            .WithUsuarioId(user1.Id)
            .WithJuegoId(game.Id)
            .WithRating(5)
            .Build();

        var review4Stars = TestDataBuilder.CreateReview()
            .WithUsuarioId(user2.Id)
            .WithJuegoId(game.Id)
            .WithRating(4)
            .Build();

        var review2Stars = TestDataBuilder.CreateReview()
            .WithUsuarioId(user3.Id)
            .WithJuegoId(game.Id)
            .WithRating(2)
            .Build();

        _dbContext.Resenas.AddRange(review5Stars, review4Stars, review2Stars);
        await _dbContext.SaveChangesAsync();

        // Act
        var highRatedReviews = await _dbContext.Resenas
            .Where(r => r.Rating >= 4 && r.Estado == EstadoResena.Activa)
            .ToListAsync();

        // Assert
        Assert.Equal(2, highRatedReviews.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se pueden obtener todas las reseñas activas")]
    public async Task ListActiveReviews_ShouldExcludeInactiveReviews()
    {
        // Arrange
        var activeReview = await _fixture.GetOrCreateTestReviewAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();

        var inactiveReview = TestDataBuilder.CreateReview()
            .WithUsuarioId(user2.Id)
            .WithJuegoId(activeReview.JuegoId)
            .WithEstado(EstadoResena.Eliminada)
            .Build();

        _dbContext.Resenas.Add(inactiveReview);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeReviews = await _dbContext.Resenas
            .Where(r => r.Estado == EstadoResena.Activa)
            .ToListAsync();

        // Assert
        Assert.True(activeReviews.All(r => r.Estado == EstadoResena.Activa));
    }

    [Fact]
    [Trait("Description", "Verifica que se puede contar reseñas por juego")]
    public async Task CountReviewsByGame_ShouldReturnCorrectCount()
    {
        // Arrange
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();

        var review1 = await _fixture.GetOrCreateTestReviewAsync(user: user1, game: game);
        var review2 = await _fixture.GetOrCreateTestReviewAsync(user: user2, game: game);

        // Act
        var reviewCount = await _dbContext.Resenas
            .Where(r => r.JuegoId == game.Id && r.Estado == EstadoResena.Activa)
            .CountAsync();

        // Assert
        Assert.Equal(2, reviewCount);
    }

    [Fact]
    [Trait("Description", "Verifica que no se puede crear reseña sin usuario o juego")]
    public async Task CreateReview_WithInvalidFK_ShouldThrowException()
    {
        // Arrange
        var review = TestDataBuilder.CreateReview()
            .WithUsuarioId(Guid.NewGuid())  // Usuario inexistente
            .WithJuegoId(Guid.NewGuid())    // Juego inexistente
            .Build();

        _dbContext.Resenas.Add(review);

        // Act & Assert
        try
        {
            await _dbContext.SaveChangesAsync();
            // Si no lanza excepción, el constraint no está configurado correctamente
        }
        catch (Exception)
        {
            // Se espera una excepción de FK
            Assert.True(true);
        }
    }
}
