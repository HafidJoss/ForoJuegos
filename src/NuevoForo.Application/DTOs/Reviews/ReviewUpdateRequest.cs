using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Reviews;

public sealed class ReviewUpdateRequest
{
    [Required]
    [StringLength(4000, MinimumLength = 5)]
    public string Texto { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }
}
