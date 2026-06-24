using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Api.IntegrationTests.Integration.Users;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Usuarios.
/// </summary>
[TestClass]
public class UsersIntegrationTests : IAsyncLifetime
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
    [Description("Verifica que se puede crear un usuario en la base de datos")]
    public async Task CreateUser_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .WithApellido("Pérez")
            .WithEmail("juan.perez@test.com")
            .Build();

        // Act
        _dbContext.Usuarios.Add(user);
        await _dbContext.SaveChangesAsync();

        // Assert
        var userSaved = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.IsNotNull(userSaved);
        Assert.AreEqual("Juan", userSaved.Nombre);
        Assert.AreEqual("Pérez", userSaved.Apellido);
        Assert.AreEqual("juan.perez@test.com", userSaved.Email);
        Assert.IsTrue(userSaved.Activo);
    }

    [TestMethod]
    [Description("Verifica que no se puede crear dos usuarios con el mismo email")]
    public async Task CreateUser_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var email = "duplicate@test.com";
        var user1 = TestDataBuilder.CreateUser()
            .WithEmail(email)
            .Build();

        var user2 = TestDataBuilder.CreateUser()
            .WithEmail(email)
            .Build();

        _dbContext.Usuarios.Add(user1);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        _dbContext.Usuarios.Add(user2);
        try
        {
            await _dbContext.SaveChangesAsync();
            Assert.Fail("Se esperaba una excepción por violación de restricción UNIQUE");
        }
        catch (Exception ex)
        {
            // Se espera una excepción de integridad
            Assert.IsTrue(ex.InnerException?.Message.Contains("duplicate") || 
                         ex.Message.Contains("duplicate") ||
                         ex.Message.Contains("unique"), 
                         "Se esperaba una excepción de constratin UNIQUE");
        }
    }

    [TestMethod]
    [Description("Verifica que se puede actualizar los datos de un usuario")]
    public async Task UpdateUser_WithModifiedData_ShouldPersistChanges()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync(
            userName: "testuser",
            email: "test@example.com"
        );

        // Act
        var userToUpdate = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        userToUpdate.Nombre = "Juan Actualizado";
        userToUpdate.Apellido = "Pérez Actualizado";
        await _dbContext.SaveChangesAsync();

        // Assert
        var userVerified = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        Assert.AreEqual("Juan Actualizado", userVerified.Nombre);
        Assert.AreEqual("Pérez Actualizado", userVerified.Apellido);
    }

    [TestMethod]
    [Description("Verifica que se puede recuperar un usuario con sus reseñas relacionadas")]
    public async Task GetUser_WithReviews_ShouldLoadRelatedEntities()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var game = await _fixture.GetOrCreateTestGameAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user.Id)
            .WithJuegoId(game.Id)
            .WithTitulo("Excelente juego")
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user.Id)
            .WithJuegoId(game.Id)
            .WithTitulo("Muy divertido")
            .Build();

        _dbContext.Resenas.AddRange(review1, review2);
        await _dbContext.SaveChangesAsync();

        // Act
        var userWithReviews = await _dbContext.Usuarios
            .Include(u => u.Resenas)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.IsNotNull(userWithReviews);
        Assert.AreEqual(2, userWithReviews.Resenas?.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede recuperar un usuario con sus comentarios relacionados")]
    public async Task GetUser_WithComments_ShouldLoadRelatedEntities()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var review = await _fixture.GetOrCreateTestReviewAsync();

        var comment1 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user,
            contenido: "Comentario 1"
        );

        var comment2 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user,
            contenido: "Comentario 2"
        );

        // Act
        var userWithComments = await _dbContext.Usuarios
            .Include(u => u.Comentarios)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.IsNotNull(userWithComments);
        Assert.AreEqual(2, userWithComments.Comentarios?.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede buscar usuarios por nombre")]
    public async Task SearchUsers_ByNombre_ShouldReturnMatches()
    {
        // Arrange
        var user1 = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .WithApellido("Pérez")
            .Build();

        var user2 = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .WithApellido("García")
            .Build();

        var user3 = TestDataBuilder.CreateUser()
            .WithNombre("Carlos")
            .WithApellido("López")
            .Build();

        _dbContext.Usuarios.AddRange(user1, user2, user3);
        await _dbContext.SaveChangesAsync();

        // Act
        var juanUsers = await _dbContext.Usuarios
            .Where(u => u.Nombre == "Juan")
            .ToListAsync();

        // Assert
        Assert.AreEqual(2, juanUsers.Count);
    }

    [TestMethod]
    [Description("Verifica que se puede desactivar un usuario (soft delete)")]
    public async Task DeactivateUser_ShouldMarkAsInactive()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();

        // Act
        var userToDeactivate = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        userToDeactivate.Activo = false;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deactivatedUser = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        Assert.IsFalse(deactivatedUser.Activo);
    }

    [TestMethod]
    [Description("Verifica que se puede contar usuarios activos")]
    public async Task CountActiveUsers_ShouldReturnCorrectCount()
    {
        // Arrange
        var activeUser = await _fixture.GetOrCreateTestUserAsync();

        var inactiveUser = TestDataBuilder.CreateUser()
            .WithActivo(false)
            .Build();

        _dbContext.Usuarios.Add(inactiveUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeCount = await _dbContext.Usuarios
            .Where(u => u.Activo)
            .CountAsync();

        // Assert
        Assert.IsTrue(activeCount >= 1);
    }
}
