using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Auth;

public sealed class UpdateProfileRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La biografía no puede exceder los 500 caracteres.")]
    public string? Bio { get; set; }
}
