using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Reviews;

public sealed class ReviewCreateRequest
{
    [Required]
    public Guid JuegoId { get; set; }

    [Required]
    [StringLength(4000, MinimumLength = 5)]
    public string Texto { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }
}
