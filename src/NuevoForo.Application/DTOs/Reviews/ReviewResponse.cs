namespace NuevoForo.Application.DTOs.Reviews;

public sealed class ReviewResponse
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid JuegoId { get; set; }
    public string? JuegoNombre { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int Likes { get; set; }
}
