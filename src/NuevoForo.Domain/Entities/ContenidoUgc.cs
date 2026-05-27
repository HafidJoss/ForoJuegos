using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class ContenidoUgc
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? JuegoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string ArchivoUrl { get; set; } = string.Empty;
    public string ArchivoNombre { get; set; } = string.Empty;
    public long ArchivoSize { get; set; }
    public string? ArchivoHash { get; set; }
    public string? FotoPath { get; set; }
    public string? FotoNombre { get; set; }
    public long? FotoSize { get; set; }
    public string? Tags { get; set; }
    public bool DeclaracionLegalAceptada { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
    public EstadoContenidoUgc Estado { get; set; } = EstadoContenidoUgc.Publicado;

    public Usuario Usuario { get; set; } = null!;
    public Juego? Juego { get; set; }
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public ICollection<LikeUgc> Likes { get; set; } = new List<LikeUgc>();
}
