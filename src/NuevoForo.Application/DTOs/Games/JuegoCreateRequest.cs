using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Games;

public sealed class JuegoCreateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Descripcion { get; set; }

    [StringLength(100)]
    public string? GeneroPrincipal { get; set; }

    [StringLength(1000)]
    public string? Tags { get; set; }

    public DateOnly? FechaLanzamiento { get; set; }

    [StringLength(100)]
    public string? Plataforma { get; set; }

    [StringLength(500)]
    public string? ImagenPortadaUrl { get; set; }
}
