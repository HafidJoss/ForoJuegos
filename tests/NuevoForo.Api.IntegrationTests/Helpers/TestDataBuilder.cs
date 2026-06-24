using NuevoForo.Domain.Entities;

namespace NuevoForo.Api.IntegrationTests.Helpers;

/// <summary>
/// Builder fluido para crear entidades de prueba de forma simple y legible.
/// Permite construcción encadenada de objetos complejos.
/// </summary>
public class TestDataBuilder
{
    /// <summary>
    /// Crea un builder para Usuario.
    /// </summary>
    public static UserBuilder CreateUser() => new();

    /// <summary>
    /// Crea un builder para Juego.
    /// </summary>
    public static GameBuilder CreateGame() => new();

    /// <summary>
    /// Crea un builder para Reseña.
    /// </summary>
    public static ReviewBuilder CreateReview() => new();

    /// <summary>
    /// Crea un builder para Comentario.
    /// </summary>
    public static CommentBuilder CreateComment() => new();

    /// <summary>
    /// Crea un builder para Like en Reseña.
    /// </summary>
    public static LikeReviewBuilder CreateLikeReview() => new();

    // ==================== USER BUILDER ====================

    public class UserBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _userName = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}";
        private string _email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@example.com";
        private string _nombre = "Test";
        private string _apellido = "User";
        private bool _emailConfirmed = true;
        private bool _activo = true;
        private DateTime _fechaCreacion = DateTime.UtcNow;

        public UserBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public UserBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserBuilder WithNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public UserBuilder WithApellido(string apellido)
        {
            _apellido = apellido;
            return this;
        }

        public UserBuilder WithEmailConfirmed(bool confirmed)
        {
            _emailConfirmed = confirmed;
            return this;
        }

        public UserBuilder WithActivo(bool activo)
        {
            _activo = activo;
            return this;
        }

        public UserBuilder WithFechaCreacion(DateTime fecha)
        {
            _fechaCreacion = fecha;
            return this;
        }

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
                Apellido = _apellido,
                EmailConfirmed = _emailConfirmed,
                FechaCreacion = _fechaCreacion,
                Activo = _activo
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
        private DateTime _fechaLanzamiento = DateTime.UtcNow.AddDays(-30);
        private DateTime _fechaCreacion = DateTime.UtcNow;
        private bool _activo = true;

        public GameBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public GameBuilder WithNombre(string nombre)
        {
            _nombre = nombre;
            return this;
        }

        public GameBuilder WithDescripcion(string descripcion)
        {
            _descripcion = descripcion;
            return this;
        }

        public GameBuilder WithPlataforma(string plataforma)
        {
            _plataforma = plataforma;
            return this;
        }

        public GameBuilder WithFechaLanzamiento(DateTime fecha)
        {
            _fechaLanzamiento = fecha;
            return this;
        }

        public GameBuilder WithFechaCreacion(DateTime fecha)
        {
            _fechaCreacion = fecha;
            return this;
        }

        public GameBuilder WithActivo(bool activo)
        {
            _activo = activo;
            return this;
        }

        public Juego Build()
        {
            return new Juego
            {
                Id = _id,
                Nombre = _nombre,
                Descripcion = _descripcion,
                Plataforma = _plataforma,
                FechaLanzamiento = _fechaLanzamiento,
                FechaCreacion = _fechaCreacion,
                Activo = _activo
            };
        }
    }

    // ==================== REVIEW BUILDER ====================

    public class ReviewBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _usuarioId = Guid.NewGuid();
        private Guid _juegoId = Guid.NewGuid();
        private string _titulo = "Test Review";
        private string _contenido = "Esta es una reseña de prueba";
        private int _calificacion = 5;
        private DateTime _fechaCreacion = DateTime.UtcNow;
        private bool _activo = true;

        public ReviewBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ReviewBuilder WithUsuarioId(Guid usuarioId)
        {
            _usuarioId = usuarioId;
            return this;
        }

        public ReviewBuilder WithJuegoId(Guid juegoId)
        {
            _juegoId = juegoId;
            return this;
        }

        public ReviewBuilder WithTitulo(string titulo)
        {
            _titulo = titulo;
            return this;
        }

        public ReviewBuilder WithContenido(string contenido)
        {
            _contenido = contenido;
            return this;
        }

        public ReviewBuilder WithCalificacion(int calificacion)
        {
            if (calificacion < 1 || calificacion > 5)
                throw new ArgumentOutOfRangeException(nameof(calificacion), "La calificación debe estar entre 1 y 5");

            _calificacion = calificacion;
            return this;
        }

        public ReviewBuilder WithFechaCreacion(DateTime fecha)
        {
            _fechaCreacion = fecha;
            return this;
        }

        public ReviewBuilder WithActivo(bool activo)
        {
            _activo = activo;
            return this;
        }

        public Resena Build()
        {
            return new Resena
            {
                Id = _id,
                UsuarioId = _usuarioId,
                JuegoId = _juegoId,
                Titulo = _titulo,
                Contenido = _contenido,
                Calificacion = _calificacion,
                FechaCreacion = _fechaCreacion,
                Activo = _activo
            };
        }
    }

    // ==================== COMMENT BUILDER ====================

    public class CommentBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _resenaId = Guid.NewGuid();
        private Guid _usuarioId = Guid.NewGuid();
        private string _contenido = "Test comment";
        private DateTime _fechaCreacion = DateTime.UtcNow;
        private bool _activo = true;

        public CommentBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CommentBuilder WithResenaId(Guid resenaId)
        {
            _resenaId = resenaId;
            return this;
        }

        public CommentBuilder WithUsuarioId(Guid usuarioId)
        {
            _usuarioId = usuarioId;
            return this;
        }

        public CommentBuilder WithContenido(string contenido)
        {
            _contenido = contenido;
            return this;
        }

        public CommentBuilder WithFechaCreacion(DateTime fecha)
        {
            _fechaCreacion = fecha;
            return this;
        }

        public CommentBuilder WithActivo(bool activo)
        {
            _activo = activo;
            return this;
        }

        public Comentario Build()
        {
            return new Comentario
            {
                Id = _id,
                ResenaId = _resenaId,
                UsuarioId = _usuarioId,
                Contenido = _contenido,
                FechaCreacion = _fechaCreacion,
                Activo = _activo
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

        public LikeReviewBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public LikeReviewBuilder WithResenaId(Guid resenaId)
        {
            _resenaId = resenaId;
            return this;
        }

        public LikeReviewBuilder WithUsuarioId(Guid usuarioId)
        {
            _usuarioId = usuarioId;
            return this;
        }

        public LikeReviewBuilder WithFechaCreacion(DateTime fecha)
        {
            _fechaCreacion = fecha;
            return this;
        }

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
