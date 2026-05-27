namespace NuevoForo.Domain.Entities;

public class LikeResena
{
    public Guid Id { get; set; }
    public Guid ResenaId { get; set; }
    public Guid UsuarioId { get; set; }
    public bool EsDislike { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Resena Resena { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
