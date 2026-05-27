using NuevoForo.Domain.Enums;

namespace NuevoForo.Domain.Entities;

public class Reporte
{
    public Guid Id { get; set; }
    public Guid ReportadoPorUsuarioId { get; set; }
    public TipoObjetivoReporte TipoObjetivo { get; set; }
    public Guid ObjetivoId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Evidencia { get; set; }
    public EstadoReporte Estado { get; set; } = EstadoReporte.Abierto;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCierre { get; set; }
    public Guid? ModeradorId { get; set; }
    public AccionModeracion AccionTomada { get; set; } = AccionModeracion.Ninguna;

    public Usuario ReportadoPorUsuario { get; set; } = null!;
    public Usuario? Moderador { get; set; }
}
