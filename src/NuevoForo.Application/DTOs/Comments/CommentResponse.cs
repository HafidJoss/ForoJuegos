namespace NuevoForo.Application.DTOs.Comments;

public sealed class CommentResponse
{
    public Guid Id { get; set; }
    public Guid? ResenaId { get; set; }
    public Guid? UgcId { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
