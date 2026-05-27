using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class Donacion
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public string ProveedorPago { get; set; } = string.Empty;
    public string TransaccionId { get; set; } = string.Empty;
    public EstadoDonacion Estado { get; set; } = EstadoDonacion.Pendiente;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Usuario Usuario { get; set; } = null!;
}
