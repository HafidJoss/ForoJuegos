namespace NuevoForo.Domain.Entities;

public class Juego
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? GeneroPrincipal { get; set; }
    public string? Tags { get; set; }
    public DateOnly? FechaLanzamiento { get; set; }
    public string? Plataforma { get; set; }
    public string? ImagenPortadaUrl { get; set; }

    public ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    public ICollection<ContenidoUgc> ContenidosUgc { get; set; } = new List<ContenidoUgc>();
}
