namespace NuevoForo.Application.DTOs.Import;

/// <summary>
/// DTO para mapear datos de juegos desde Steam Web API Dump.
/// Contiene los campos principales de un juego Steam que serán importados a la BD.
/// </summary>
public sealed class SteamGameDto
{
    /// <summary>
    /// ID único del juego en Steam (AppID).
    /// Se usa como referencia de Steam, no se almacena en BD.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Nombre del juego. Máximo 200 caracteres.
    /// Ejemplo: "Dota 2", "Elden Ring", "Portal 2"
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Descripción corta del juego.
    /// Máximo 2000 caracteres.
    /// Ejemplo: "A free-to-play team-based game..."
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Géneros del juego separados por comas.
    /// Ejemplo: "Action, RPG, Strategy"
    /// Se divide en GeneroPrincipal (primero) y Tags (todos).
    /// </summary>
    public string? Genres { get; set; }

    /// <summary>
    /// Fecha de lanzamiento del juego.
    /// Formato: "2023-12-25" o "Dec 25, 2023"
    /// Se convierte a DateOnly para la entidad Juego.
    /// </summary>
    public string? ReleaseDate { get; set; }

    /// <summary>
    /// URL de la imagen de portada (header image) del juego.
    /// Ejemplo: "https://cdn.akamai.steamstatic.com/steam/apps/570/header.jpg"
    /// Se valida antes de guardar.
    /// </summary>
    public string? HeaderImage { get; set; }

    /// <summary>
    /// Plataformas soportadas del juego.
    /// Formato JSON: { "windows": true, "mac": false, "linux": true }
    /// Se convierte a string: "Windows, Linux"
    /// </summary>
    public string? Platforms { get; set; }

    /// <summary>
    /// URL del sitio web del juego (opcional).
    /// Ejemplo: "http://www.dota2.com"
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Precio del juego (puede ser "Free To Play", "$49.99", etc.)
    /// No se almacena en BD, solo para referencia.
    /// </summary>
    public string? Price { get; set; }

    /// <summary>
    /// Nota de la comunidad en Metacritic (0-100).
    /// No se almacena en BD.
    /// </summary>
    public int? MetacriticScore { get; set; }
}
