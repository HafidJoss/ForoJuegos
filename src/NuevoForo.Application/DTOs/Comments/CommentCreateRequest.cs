using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Comments;

public sealed class CommentCreateRequest : IValidatableObject
{
    public Guid? ResenaId { get; set; }
    
    public Guid? UgcId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 2)]
    public string Texto { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ResenaId is null && UgcId is null)
        {
            yield return new ValidationResult("Debe especificar ResenaId o UgcId.", new[] { nameof(ResenaId), nameof(UgcId) });
        }
        
        if (ResenaId is not null && UgcId is not null)
        {
            yield return new ValidationResult("Un comentario no puede pertenecer a una reseña y a un UGC al mismo tiempo.", new[] { nameof(ResenaId), nameof(UgcId) });
        }
    }
}
