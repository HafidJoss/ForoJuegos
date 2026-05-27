using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class Comentario
{
    public Guid Id { get; set; }
    public Guid? ResenaId { get; set; }
    public Guid? UgcId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public EstadoComentario Estado { get; set; } = EstadoComentario.Activo;

    public Resena? Resena { get; set; }
    public ContenidoUgc? Ugc { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
