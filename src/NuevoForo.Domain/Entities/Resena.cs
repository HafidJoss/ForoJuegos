using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class Resena
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JuegoId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
    public EstadoResena Estado { get; set; } = EstadoResena.Activa;

    public Usuario Usuario { get; set; } = null!;
    public Juego Juego { get; set; } = null!;
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public ICollection<LikeResena> Likes { get; set; } = new List<LikeResena>();
}
