using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Ugc;

public sealed class UgcUpdateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Descripcion { get; set; }

    [StringLength(1000)]
    public string? Tags { get; set; }

    [StringLength(1000, MinimumLength = 5)]
    public string? ArchivoUrl { get; set; }
}
