using NuevoForo.Domain.Entities;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.IntegrationTests.Helpers;

/// <summary>
/// Builder fluido para crear entidades de prueba de forma simple y legible.
/// Permite construcción encadenada de objetos complejos.
/// </summary>
public class TestDataBuilder
{
    public static UserBuilder CreateUser() => new();
    public static GameBuilder CreateGame() => new();
    public static ReviewBuilder CreateReview() => new();
    public static CommentBuilder CreateComment() => new();
    public static LikeReviewBuilder CreateLikeReview() => new();

    // ==================== USER BUILDER ====================
    public class UserBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _userName = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}";
        private string _email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
        private string _nombre = "Test";
        private bool _emailConfirmed = true;
        private EstadoUsuario _estado = EstadoUsuario.Activo;
        private DateTime _fechaRegistro = DateTime.UtcNow;

        public UserBuilder WithId(Guid id) { _id = id; return this; }
        public UserBuilder WithUserName(string userName) { _userName = userName; return this; }
        public UserBuilder WithEmail(string email) { _email = email; return this; }
        public UserBuilder WithNombre(string nombre) { _nombre = nombre; return this; }
        public UserBuilder WithEmailConfirmed(bool confirmed) { _emailConfirmed = confirmed; return this; }
        public UserBuilder WithEstado(EstadoUsuario estado) { _estado = estado; return this; }
        public UserBuilder WithFechaRegistro(DateTime fecha) { _fechaRegistro = fecha; return this; }

        public Usuario Build()
        {
            return new Usuario
            {
                Id = _id,
                UserName = _userName,
                NormalizedUserName = _userName.ToUpper(),
                Email = _email,
                NormalizedEmail = _email.ToUpper(),
                Nombre = _nombre,
                EmailConfirmed = _emailConfirmed,
                FechaRegistro = _fechaRegistro,
                Estado = _estado,
                PasswordHash = "AQAAAAEAACcQAAAAEA...", // Dummy hash
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
        }
    }

    // ==================== GAME BUILDER ====================
    public class GameBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _nombre = $"Test Game {Guid.NewGuid().ToString().Substring(0, 8)}";
        private string _descripcion = "Juego de prueba";
        private string _plataforma = "PC";
        private DateOnly _fechaLanzamiento = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

        public GameBuilder WithId(Guid id) { _id = id; return this; }
        public GameBuilder WithNombre(string nombre) { _nombre = nombre; return this; }
        public GameBuilder WithDescripcion(string descripcion) { _descripcion = descripcion; return this; }
        public GameBuilder WithPlataforma(string plataforma) { _plataforma = plataforma; return this; }
        public GameBuilder WithFechaLanzamiento(DateOnly fecha) { _fechaLanzamiento = fecha; return this; }

        public Juego Build()
        {
            return new Juego
            {
                Id = _id,
                Nombre = _nombre,
                Descripcion = _descripcion,
                Plataforma = _plataforma,
                FechaLanzamiento = _fechaLanzamiento
            };
        }
    }

    // ==================== REVIEW BUILDER ====================
    public class ReviewBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _usuarioId = Guid.NewGuid();
        private Guid _juegoId = Guid.NewGuid();
        private string _texto = "Esta es una reseña de prueba";
        private int _rating = 5;
        private DateTime _fechaCreacion = DateTime.UtcNow;
        private EstadoResena _estado = EstadoResena.Activa;

        public ReviewBuilder WithId(Guid id) { _id = id; return this; }
        public ReviewBuilder WithUsuarioId(Guid usuarioId) { _usuarioId = usuarioId; return this; }
        public ReviewBuilder WithJuegoId(Guid juegoId) { _juegoId = juegoId; return this; }
        public ReviewBuilder WithTexto(string texto) { _texto = texto; return this; }
        public ReviewBuilder WithRating(int rating)
        {
            if (rating < 1 || rating > 10)
                throw new ArgumentOutOfRangeException(nameof(rating), "El rating debe estar entre 1 y 10");
            _rating = rating;
            return this;
        }
        public ReviewBuilder WithFechaCreacion(DateTime fecha) { _fechaCreacion = fecha; return this; }
        public ReviewBuilder WithEstado(EstadoResena estado) { _estado = estado; return this; }

        public Resena Build()
        {
            return new Resena
            {
                Id = _id,
                UsuarioId = _usuarioId,
                JuegoId = _juegoId,
                Texto = _texto,
                Rating = _rating,
                FechaCreacion = _fechaCreacion,
                Estado = _estado
            };
        }
    }

    // ==================== COMMENT BUILDER ====================
    public class CommentBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _resenaId = Guid.NewGuid();
        private Guid _usuarioId = Guid.NewGuid();
        private string _texto = "Test comment";
        private DateTime _fechaCreacion = DateTime.UtcNow;
        private EstadoComentario _estado = EstadoComentario.Activo;

        public CommentBuilder WithId(Guid id) { _id = id; return this; }
        public CommentBuilder WithResenaId(Guid resenaId) { _resenaId = resenaId; return this; }
        public CommentBuilder WithUsuarioId(Guid usuarioId) { _usuarioId = usuarioId; return this; }
        public CommentBuilder WithTexto(string texto) { _texto = texto; return this; }
        public CommentBuilder WithFechaCreacion(DateTime fecha) { _fechaCreacion = fecha; return this; }
        public CommentBuilder WithEstado(EstadoComentario estado) { _estado = estado; return this; }

        public Comentario Build()
        {
            return new Comentario
            {
                Id = _id,
                ResenaId = _resenaId,
                UsuarioId = _usuarioId,
                Texto = _texto,
                FechaCreacion = _fechaCreacion,
                Estado = _estado
            };
        }
    }

    // ==================== LIKE REVIEW BUILDER ====================
    public class LikeReviewBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _resenaId = Guid.NewGuid();
        private Guid _usuarioId = Guid.NewGuid();
        private DateTime _fechaCreacion = DateTime.UtcNow;

        public LikeReviewBuilder WithId(Guid id) { _id = id; return this; }
        public LikeReviewBuilder WithResenaId(Guid resenaId) { _resenaId = resenaId; return this; }
        public LikeReviewBuilder WithUsuarioId(Guid usuarioId) { _usuarioId = usuarioId; return this; }
        public LikeReviewBuilder WithFechaCreacion(DateTime fecha) { _fechaCreacion = fecha; return this; }

        public LikeResena Build()
        {
            return new LikeResena
            {
                Id = _id,
                ResenaId = _resenaId,
                UsuarioId = _usuarioId,
                FechaCreacion = _fechaCreacion
            };
        }
    }
}
