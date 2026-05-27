using NuevoForo.Domain.Enums;

namespace NuevoForo.Application.DTOs.Reports;

public sealed class ReportResponse
{
    public Guid Id { get; set; }
    public Guid ReportadoPorUsuarioId { get; set; }
    public TipoObjetivoReporte TipoObjetivo { get; set; }
    public Guid ObjetivoId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Evidencia { get; set; }
    public EstadoReporte Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public Guid? ModeradorId { get; set; }
    public AccionModeracion AccionTomada { get; set; }
}
