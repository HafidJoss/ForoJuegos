using System.ComponentModel.DataAnnotations;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Application.DTOs.Reports;

public sealed class ReportCreateRequest
{
    [Required]
    public TipoObjetivoReporte TipoObjetivo { get; set; }

    [Required]
    public Guid ObjetivoId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Motivo { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Evidencia { get; set; }
}
