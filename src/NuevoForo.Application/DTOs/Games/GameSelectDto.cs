namespace NuevoForo.Application.DTOs.Games;

/// <summary>
/// DTO simplificado para selector de juegos en componentes frontend.
/// Contiene solo la información necesaria para mostrar juegos en un dropdown.
/// </summary>
public sealed class GameSelectDto
{
    /// <summary>Identificador único del juego (Guid).</summary>
    public Guid Id { get; set; }

    /// <summary>Nombre del juego.</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>URL de la imagen de portada del juego (thumbnail).</summary>
    public string? ImagenPortadaUrl { get; set; }
}
