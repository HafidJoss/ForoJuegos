using System.ComponentModel.DataAnnotations;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Application.DTOs.Reports;

public sealed class ModerationActionRequest
{
    [Required]
    public AccionModeracion AccionTomada { get; set; }

    [StringLength(500)]
    public string? ComentarioOpcional { get; set; }

    public bool Rechazar { get; set; }
}
