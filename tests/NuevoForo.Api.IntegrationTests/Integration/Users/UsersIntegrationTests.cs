using Microsoft.EntityFrameworkCore;
using Xunit;
using NuevoForo.Api.IntegrationTests.Fixtures;
using NuevoForo.Api.IntegrationTests.Helpers;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.IntegrationTests.Integration.Users;

/// <summary>
/// Pruebas de integración para operaciones CRUD y relaciones de Usuarios.
/// </summary>
public class UsersIntegrationTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private AppDbContext _dbContext = null!;

    public UsersIntegrationTests(DatabaseFixture fixture)
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
    [Trait("Description", "Verifica que se puede crear un usuario en la base de datos")]
    public async Task CreateUser_WithValidData_ShouldPersistInDatabase()
    {
        // Arrange
        var user = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .WithEmail("juan.perez@test.com")
            .Build();

        // Act
        _dbContext.Usuarios.Add(user);
        await _dbContext.SaveChangesAsync();

        // Assert
        var userSaved = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        Assert.NotNull(userSaved);
        Assert.Equal("Juan", userSaved.Nombre);
        Assert.Equal("juan.perez@test.com", userSaved.Email);
        Assert.Equal(EstadoUsuario.Activo, userSaved.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que no se puede crear dos usuarios con el mismo email")]
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
            Assert.True(ex.InnerException?.Message.Contains("duplicate") == true || 
                         ex.Message.Contains("duplicate") ||
                         ex.Message.Contains("unique"), 
                         "Se esperaba una excepción de constratin UNIQUE");
        }
    }

    [Fact]
    [Trait("Description", "Verifica que se puede actualizar los datos de un usuario")]
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
        await _dbContext.SaveChangesAsync();

        // Assert
        var userVerified = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        Assert.Equal("Juan Actualizado", userVerified.Nombre);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar un usuario con sus reseñas relacionadas")]
    public async Task GetUser_WithReviews_ShouldLoadRelatedEntities()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var game1 = await _fixture.GetOrCreateTestGameAsync();
        var game2 = await _fixture.GetOrCreateTestGameAsync();

        var review1 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user.Id)
            .WithJuegoId(game1.Id)
            .WithTexto("Excelente juego")
            .Build();

        var review2 = TestDataBuilder.CreateReview()
            .WithUsuarioId(user.Id)
            .WithJuegoId(game2.Id)
            .WithTexto("Muy divertido")
            .Build();

        _dbContext.Resenas.AddRange(review1, review2);
        await _dbContext.SaveChangesAsync();

        // Act
        var userWithReviews = await _dbContext.Usuarios
            .Include(u => u.Resenas)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.NotNull(userWithReviews);
        Assert.Equal(2, userWithReviews.Resenas?.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede recuperar un usuario con sus comentarios relacionados")]
    public async Task GetUser_WithComments_ShouldLoadRelatedEntities()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();
        var review = await _fixture.GetOrCreateTestReviewAsync();

        var comment1 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user,
            texto: "Comentario 1"
        );

        var comment2 = await _fixture.GetOrCreateTestCommentAsync(
            review: review,
            user: user,
            texto: "Comentario 2"
        );

        // Act
        var userWithComments = await _dbContext.Usuarios
            .Include(u => u.Comentarios)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.NotNull(userWithComments);
        Assert.Equal(2, userWithComments.Comentarios?.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede buscar usuarios por nombre")]
    public async Task SearchUsers_ByNombre_ShouldReturnMatches()
    {
        // Arrange
        var user1 = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .Build();

        var user2 = TestDataBuilder.CreateUser()
            .WithNombre("Juan")
            .Build();

        var user3 = TestDataBuilder.CreateUser()
            .WithNombre("Carlos")
            .Build();

        _dbContext.Usuarios.AddRange(user1, user2, user3);
        await _dbContext.SaveChangesAsync();

        // Act
        var juanUsers = await _dbContext.Usuarios
            .Where(u => u.Nombre == "Juan")
            .ToListAsync();

        // Assert
        Assert.Equal(2, juanUsers.Count);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede desactivar un usuario (soft delete)")]
    public async Task DeactivateUser_ShouldMarkAsInactive()
    {
        // Arrange
        var user = await _fixture.GetOrCreateTestUserAsync();

        // Act
        var userToDeactivate = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        userToDeactivate.Estado = EstadoUsuario.Suspendido;
        await _dbContext.SaveChangesAsync();

        // Assert
        var deactivatedUser = await _dbContext.Usuarios
            .FirstAsync(u => u.Id == user.Id);

        Assert.Equal(EstadoUsuario.Suspendido, deactivatedUser.Estado);
    }

    [Fact]
    [Trait("Description", "Verifica que se puede contar usuarios activos")]
    public async Task CountActiveUsers_ShouldReturnCorrectCount()
    {
        // Arrange
        var activeUser = await _fixture.GetOrCreateTestUserAsync();

        var inactiveUser = TestDataBuilder.CreateUser()
            .WithEstado(EstadoUsuario.Suspendido)
            .Build();

        _dbContext.Usuarios.Add(inactiveUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeCount = await _dbContext.Usuarios
            .Where(u => u.Estado == EstadoUsuario.Activo)
            .CountAsync();

        // Assert
        Assert.True(activeCount >= 1);
    }
}
