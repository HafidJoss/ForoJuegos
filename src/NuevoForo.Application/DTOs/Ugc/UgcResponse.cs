namespace NuevoForo.Application.DTOs.Ugc;

public sealed class UgcResponse
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? JuegoId { get; set; }
    public string? JuegoNombre { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string ArchivoUrl { get; set; } = string.Empty;
    public string? ArchivoNombre { get; set; }
    public long ArchivoSize { get; set; }
    public string? FotoPath { get; set; }
    public string? FotoNombre { get; set; }
    public long? FotoSize { get; set; }
    public string? Tags { get; set; }
    public bool DeclaracionLegalAceptada { get; set; }
    public DateTime FechaSubida { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
