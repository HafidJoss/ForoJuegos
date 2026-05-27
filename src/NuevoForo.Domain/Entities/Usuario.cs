using Microsoft.AspNetCore.Identity;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class Usuario : IdentityUser<Guid>
{
    public string Nombre { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Idioma { get; set; }
    public RolUsuario Rol { get; set; } = RolUsuario.Usuario;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public EstadoUsuario Estado { get; set; } = EstadoUsuario.Activo;
    public DateTime? UltimoLogin { get; set; }

    public ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public ICollection<LikeResena> LikesResena { get; set; } = new List<LikeResena>();
    public ICollection<ContenidoUgc> ContenidosUgc { get; set; } = new List<ContenidoUgc>();
    public ICollection<Reporte> ReportesRealizados { get; set; } = new List<Reporte>();
    public ICollection<Reporte> ReportesModerados { get; set; } = new List<Reporte>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public ICollection<Donacion> Donaciones { get; set; } = new List<Donacion>();
}
