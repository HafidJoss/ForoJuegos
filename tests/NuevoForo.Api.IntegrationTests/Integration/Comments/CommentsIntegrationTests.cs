using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Api.IntegrationTests.Integration.Comments;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Comentarios.
/// </summary>
[TestClass]
public class CommentsIntegrationTests : IAsyncLifetime
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
    [Description("Verifica que se puede crear un comentario en una reseña")]
    public async Task CreateComment_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var comment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Totalmente de acuerdo, excelente reseña")
            .Build();

        // Act
        _dbContext.Comentarios.Add(comment);
        await _dbContext.SaveChangesAsync();

        // Assert
        var commentSaved = await _dbContext.Comentarios
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        Assert.IsNotNull(commentSaved);
        Assert.AreEqual(review.Id, commentSaved.ResenaId);
        Assert.AreEqual(user.Id, commentSaved.UsuarioId);
        Assert.AreEqual("Totalmente de acuerdo, excelente reseña", commentSaved.Contenido);
        Assert.IsTrue(commentSaved.Activo);
    }

    [TestMethod]
    [Description("Verifica que se puede actualizar un comentario")]
    public async Task UpdateComment_WithModifiedData_ShouldPersistChanges()
    {
        // Arrange
        var comment = await _fixture.GetOrCreateTestCommentAsync(
            contenido: "Comentario original"
        );

        // Act
        var commentToUpdate = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        commentToUpdate.Contenido = "Comentario actualizado";
        await _dbContext.SaveChangesAsync();

        // Assert
        var commentVerified = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        Assert.AreEqual("Comentario actualizado", commentVerified.Contenido);
    }

    [TestMethod]
    [Description("Verifica que se puede eliminar (soft delete) un comentario")]
    public async Task DeleteComment_ShouldMarkAsInactive()
    {
        // Arrange
        var comment = await _fixture.GetOrCreateTestCommentAsync();

        // Act
        var commentToDelete = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        commentToDelete.Activo = false;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedComment = await _dbContext.Comentarios
            .FirstAsync(c => c.Id == comment.Id);

        Assert.IsFalse(deletedComment.Activo);
    }

    [TestMethod]
    [Description("Verifica que se puede recuperar un comentario con usuario relacionado")]
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
        Assert.IsNotNull(commentWithUser);
        Assert.IsNotNull(commentWithUser.Usuario);
        Assert.AreEqual(user.Id, commentWithUser.Usuario.Id);
    }

    [TestMethod]
    [Description("Verifica que se puede recuperar un comentario con reseña relacionada")]
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
        Assert.IsNotNull(commentWithReview);
        Assert.IsNotNull(commentWithReview.Resena);
        Assert.AreEqual(review.Id, commentWithReview.Resena.Id);
    }

    [TestMethod]
    [Description("Verifica que se puede buscar comentarios por contenido")]
    public async Task SearchComments_ByContenido_ShouldReturnMatches()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user1 = await _fixture.GetOrCreateTestUserAsync();
        var user2 = await _fixture.GetOrCreateTestUserAsync();

        var comment1 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user1,
            contenido: "Excelente análisis del juego"
        );

        var comment2 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user2,
            contenido: "No estoy de acuerdo con el análisis"
        );

        var comment3 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user1,
            contenido: "Las gráficas son muy buenas"
        );

        // Act
        var analysisComments = await _dbContext.Comentarios
            .Where(c => c.Contenido.Contains("análisis") && c.Activo)
            .ToListAsync();

        // Assert
        Assert.AreEqual(2, analysisComments.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede contar comentarios en una reseña")]
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
            .Where(c => c.ResenaId == review.Id && c.Activo)
            .CountAsync();

        // Assert
        Assert.AreEqual(3, commentCount);
    }

    [TestMethod]
    [Description("Verifica que se pueden obtener comentarios ordenados por fecha")]
    public async Task GetComments_OrderedByFecha_ShouldReturnInCorrectOrder()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var comment1 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Primer comentario")
            .WithFechaCreacion(DateTime.UtcNow.AddHours(-2))
            .Build();

        var comment2 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Segundo comentario")
            .WithFechaCreacion(DateTime.UtcNow.AddHours(-1))
            .Build();

        var comment3 = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Tercer comentario")
            .WithFechaCreacion(DateTime.UtcNow)
            .Build();

        _dbContext.Comentarios.AddRange(comment1, comment2, comment3);
        await _dbContext.SaveChangesAsync();

        // Act
        var orderedComments = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Activo)
            .OrderBy(c => c.FechaCreacion)
            .ToListAsync();

        // Assert
        Assert.AreEqual(3, orderedComments.Count);
        Assert.AreEqual("Primer comentario", orderedComments[0].Contenido);
        Assert.AreEqual("Tercer comentario", orderedComments[2].Contenido);
    }

    [TestMethod]
    [Description("Verifica que solo se obtienen comentarios activos")]
    public async Task ListActiveComments_ShouldExcludeInactiveComments()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();

        var activeComment = await _fixture.GetOrCreateTestCommentAsync(review: review);

        var inactiveComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(activeComment.UsuarioId)
            .WithActivo(false)
            .Build();

        _dbContext.Comentarios.Add(inactiveComment);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeComments = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Activo)
            .ToListAsync();

        // Assert
        Assert.IsTrue(activeComments.All(c => c.Activo));
    }

    [TestMethod]
    [Description("Verifica que se puede obtener el comentario más reciente de una reseña")]
    public async Task GetLatestComment_ByReview_ShouldReturnMostRecent()
    {
        // Arrange
        var review = await _fixture.GetOrCreateTestReviewAsync();
        var user = await _fixture.GetOrCreateTestUserAsync();

        var oldComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Comentario antiguo")
            .WithFechaCreacion(DateTime.UtcNow.AddDays(-1))
            .Build();

        var newComment = TestDataBuilder.CreateComment()
            .WithResenaId(review.Id)
            .WithUsuarioId(user.Id)
            .WithContenido("Comentario reciente")
            .WithFechaCreacion(DateTime.UtcNow)
            .Build();

        _dbContext.Comentarios.AddRange(oldComment, newComment);
        await _dbContext.SaveChangesAsync();

        // Act
        var latestComment = await _dbContext.Comentarios
            .Where(c => c.ResenaId == review.Id && c.Activo)
            .OrderByDescending(c => c.FechaCreacion)
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(latestComment);
        Assert.AreEqual("Comentario reciente", latestComment.Contenido);
    }
}
