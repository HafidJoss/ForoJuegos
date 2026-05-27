using System;

namespace NuevoForo.Domain.Entities;

public class LikeUgc
{
    public Guid Id { get; set; }
    public Guid UgcId { get; set; }
    public Guid UsuarioId { get; set; }
    public bool EsDislike { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ContenidoUgc Ugc { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
