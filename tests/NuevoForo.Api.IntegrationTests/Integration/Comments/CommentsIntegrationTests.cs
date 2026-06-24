using Microsoft.EntityFrameworkCore;
using Xunit;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.IntegrationTests.Integration.Comments;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Comentarios.
/// </summary>
public class CommentsIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private AppDbContext _dbContext = null!;

    public CommentsIntegrationTests(DatabaseFixture fixture)
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
    [Trait("Description", "Verifica que se puede crear un comentario en una reseña")]
    public async Task CreateComment_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var comment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Totalmente de acuerdo, excelente reseña")
            .Build();

        // Act
        _dbContext.Comentarios.Add(comment);
        await _dbContext.SaveChangesAsync();

        // Assert
        var commentSaved = await _dbContext.Comentarios
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        Assert.NotNull(commentSaved);
        Assert.Equal(review.Id, commentSaved.ResenaId);
        Assert.Equal(user.Id, commentSaved.UsuarioId);
        Assert.Equal("Totalmente de acuerdo, excelente reseña", commentSaved.Texto);
        Assert.Equal(EstadoComentario.Activo, commentSaved.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede actualizar un comentario")]
    public async Task UpdateComment_WithModifiedData_ShouldPersistChanges()
    {
        // Arrange
        var comment = await _fixture.GetOrCreateTestCommentAsync(
            texto: "Comentario original"
        );

        // Act
        var commentToUpdate = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        commentToUpdate.Texto = "Comentario actualizado";
        await _dbContext.SaveChangesAsync();

        // Assert
        var commentVerified = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        Assert.Equal("Comentario actualizado", commentVerified.Texto);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede eliminar (soft delete) un comentario")]
    public async Task DeleteComment_ShouldMarkAsInactive()
    {
        // Arrange
        var comment = await _fixture.GetOrCreateTestCommentAsync();

        // Act
        var commentToDelete = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        commentToDelete.Estado = EstadoComentario.Eliminado;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedComment = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        Assert.Equal(EstadoComentario.Eliminado, deletedComment.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar un comentario con usuario relacionado")]
    public async Task GetComment_WithUser_ShouldLoadRelatedEntity()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var comment = await _fixture.GetOrCreateTestCommentAsync(user: user);

        // Act
        var commentWithUser = await _dbContext.Comentarios
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        // Assert
        Assert.NotNull(commentWithUser);
        Assert.NotNull(commentWithUser.Usuario);
        Assert.Equal(user.Id, commentWithUser.Usuario.Id);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar un comentario con reseña relacionada")]
    public async Task GetComment_WithReview_ShouldLoadRelatedEntity()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var comment = await _fixture.GetOrCreateTestCommentAsync(review: review);

        // Act
        var commentWithReview = await _dbContext.Comentarios
            .Include(c => c.Resena)
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        // Assert
        Assert.NotNull(commentWithReview);
        Assert.NotNull(commentWithReview.Resena);
        Assert.Equal(review.Id, commentWithReview.Resena.Id);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede buscar comentarios por texto")]
    public async Task SearchComments_ByTexto_ShouldReturnMatches()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();

        var comment1 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user1,
            texto: "Excelente análisis del juego"
        );

        var comment2 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user2,
            texto: "No estoy de acuerdo con el análisis"
        );

        var comment3 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user1,
            texto: "Las gráficas son muy buenas"
        );

        // Act
        var analysisComments = await _dbContext.Comentarios
            .Where(c => c.Texto.Contains("análisis") && c.Estado == EstadoComentario.Activo)
            .ToListAsync();

        // Assert
        Assert.Equal(2, analysisComments.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede contar comentarios en una reseña")]
    public async Task CountCommentsByReview_ShouldReturnCorrectCount()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();
        var user3 = await _fixture.GetOrCreateTestUserAsync();

        await _fixture.GetOrCreateTestCommentAsync(review: review, user: user1);
        await _fixture.GetOrCreateTestCommentAsync(review: review, user: user2);
        await _fixture.GetOrCreateTestCommentAsync(review: review, user: user3);

        // Act
        var commentCount = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Estado == EstadoComentario.Activo)
            .CountAsync();

        // Assert
        Assert.Equal(3, commentCount);
    }

    [Fact]
    [Trait("Description", "Verifica que se pueden obtener comentarios ordenados por fecha")]
    public async Task GetComments_OrderedByFecha_ShouldReturnInCorrectOrder()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var comment1 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Primer comentario")
            .WithFechaCreacion(DateTime.UtcNow.AddHours(-2))
            .Build();

        var comment2 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Segundo comentario")
            .WithFechaCreacion(DateTime.UtcNow.AddHours(-1))
            .Build();

        var comment3 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Tercer comentario")
            .WithFechaCreacion(DateTime.UtcNow)
            .Build();

        _dbContext.Comentarios.AddRange(comment1, comment2, comment3);
        await _dbContext.SaveChangesAsync();

        // Act
        var orderedComments = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Estado == EstadoComentario.Activo)
            .OrderBy(c => c.FechaCreacion)
            .ToListAsync();

        // Assert
        Assert.Equal(3, orderedComments.Count);
        Assert.Equal("Primer comentario", orderedComments[0].Texto);
        Assert.Equal("Tercer comentario", orderedComments[2].Texto);
    }

    [Fact]
    [Trait("Description", "Verifica que solo se obtienen comentarios activos")]
    public async Task ListActiveComments_ShouldExcludeInactiveComments()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();

        var activeComment = await _fixture.GetOrCreateTestCommentAsync(review: review);

        var inactiveComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(activeComment.UsuarioId)
            .WithEstado(EstadoComentario.Eliminado)
            .Build();

        _dbContext.Comentarios.Add(inactiveComment);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeComments = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Estado == EstadoComentario.Activo)
            .ToListAsync();

        // Assert
        Assert.True(activeComments.All(c => c.Estado == EstadoComentario.Activo));
    }

    [Fact]
    [Trait("Description", "Verifica que se puede obtener el comentario más reciente de una reseña")]
    public async Task GetLatestComment_ByReview_ShouldReturnMostRecent()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var oldComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Comentario antiguo")
            .WithFechaCreacion(DateTime.UtcNow.AddDays(-1))
            .Build();

        var newComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithTexto("Comentario reciente")
            .WithFechaCreacion(DateTime.UtcNow)
            .Build();

        _dbContext.Comentarios.AddRange(oldComment, newComment);
        await _dbContext.SaveChangesAsync();

        // Act
        var latestComment = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Estado == EstadoComentario.Activo)
            .OrderByDescending(c => c.FechaCreacion)
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(latestComment);
        Assert.Equal("Comentario reciente", latestComment.Texto);
    }
}
