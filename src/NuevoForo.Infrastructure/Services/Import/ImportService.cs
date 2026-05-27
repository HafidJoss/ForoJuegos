using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuevoForo.Application.DTOs.Import;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services.Import;

/// <summary>
/// Servicio para importar juegos desde Steam Web API Dump.
/// Maneja descarga, validación, parseo, transformación y persistencia de datos.
/// </summary>
public sealed class ImportService : IImportService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ImportService> _logger;
    private readonly HttpClient _httpClient;

    // Constantes de configuración
    private const string GitHubRawUrl = "https://raw.githubusercontent.com/Splitwire/steam-api-dump/main/";
    private const string GamesFile = "appdata/all_games.json";
    private const int RequestTimeoutSeconds = 30;

    public ImportService(
        AppDbContext dbContext,
        ILogger<ImportService> logger,
        HttpClient httpClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Importa juegos desde un archivo JSON de Steam Web API Dump.
    /// </summary>
    public async Task<ImportResult> ImportFromSteamDumpAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = new ImportResult { StartTime = DateTime.UtcNow };

        try
        {
            if (!File.Exists(filePath))
            {
                var error = $"Archivo no encontrado: {filePath}";
                _logger.LogError(error);
                result.AddError(error);
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            _logger.LogInformation("Iniciando importación desde: {FilePath}", filePath);

            // Leer y parsear JSON
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var steamGames = JsonSerializer.Deserialize<List<SteamGameDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (steamGames == null || steamGames.Count == 0)
            {
                var error = "No se encontraron juegos en el archivo JSON";
                _logger.LogWarning(error);
                result.AddError(error);
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            result.Total = steamGames.Count;
            _logger.LogInformation("Se encontraron {Total} juegos para importar", result.Total);

            // Obtener IDs existentes para evitar duplicados
            var existingNames = await _dbContext.Juegos
                .Select(j => j.Nombre)
                .ToListAsync();
            var existingNamesSet = new HashSet<string>(existingNames);

            // Procesar cada juego
            foreach (var steamGame in steamGames)
            {
                try
                {
                    // Validación básica
                    if (string.IsNullOrWhiteSpace(steamGame.Name))
                    {
                        result.Duplicates++;
                        continue;
                    }

                    // Verificar duplicados
                    if (existingNamesSet.Contains(steamGame.Name))
                    {
                        result.Duplicates++;
                        continue;
                    }

                    // Mapear DTO a entidad
                    var juego = MapSteamGameToJuego(steamGame);

                    if (juego == null)
                    {
                        result.AddError($"No se pudo mapear el juego: {steamGame.Name}");
                        continue;
                    }

                    // Agregar a contexto
                    _dbContext.Juegos.Add(juego);
                    existingNamesSet.Add(juego.Nombre);
                    result.Inserted++;
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Error procesando juego '{steamGame.Name}': {ex.Message}";
                    _logger.LogError(ex, errorMsg);
                    result.AddError(errorMsg);
                }
            }

            // Guardar todos los cambios en la BD
            if (result.Inserted > 0)
            {
                try
                {
                    var saved = await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Importación completada: {Saved} registros guardados", saved);
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Error al guardar en la BD: {ex.Message}";
                    _logger.LogError(ex, errorMsg);
                    result.AddError(errorMsg);
                    result.Inserted = 0; // Marcar como falló si no se guardó
                }
            }
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error durante la importación: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            result.AddError(errorMsg);
        }
        finally
        {
            result.EndTime = DateTime.UtcNow;
            _logger.LogInformation("Resultado de importación: {Summary}", result.Summary);
        }

        return result;
    }

    /// <summary>
    /// Descarga el archivo de Steam Web API Dump desde GitHub.
    /// </summary>
    public async Task<bool> DownloadSteamDumpAsync(
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Crear directorio si no existe
            var directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Si ya existe, no descargar de nuevo (optimización)
            if (File.Exists(destinationPath))
            {
                _logger.LogInformation("Archivo Steam dump ya existe: {Path}", destinationPath);
                return true;
            }

            var downloadUrl = $"{GitHubRawUrl}{GamesFile}";
            _logger.LogInformation("Descargando Steam dump desde: {Url}", downloadUrl);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(RequestTimeoutSeconds));

            using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseContentRead, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Descarga fallida. Status: {StatusCode}", response.StatusCode);
                return false;
            }

            // Guardar archivo
            await using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
            await using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                await contentStream.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Steam dump descargado exitosamente: {Path}", destinationPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar Steam dump");
            return false;
        }
    }

    /// <summary>
    /// Valida la estructura de un archivo JSON de Steam dump.
    /// </summary>
    public async Task<List<string>> ValidateSteamDumpAsync(string filePath)
    {
        var errors = new List<string>();

        try
        {
            if (!File.Exists(filePath))
            {
                errors.Add($"Archivo no encontrado: {filePath}");
                return errors;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var steamGames = JsonSerializer.Deserialize<List<SteamGameDto>>(
                json,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            if (steamGames == null)
            {
                errors.Add("El JSON no es un array válido de juegos");
                return errors;
            }

            if (steamGames.Count == 0)
            {
                errors.Add("El archivo no contiene juegos");
                return errors;
            }

            // Validar estructura de algunos registros
            var sampleSize = Math.Min(10, steamGames.Count);
            for (int i = 0; i < sampleSize; i++)
            {
                var game = steamGames[i];
                if (string.IsNullOrWhiteSpace(game.Name))
                {
                    errors.Add($"Juego en índice {i} no tiene nombre");
                }
            }
        }
        catch (JsonException ex)
        {
            errors.Add($"Error al parsear JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            errors.Add($"Error durante validación: {ex.Message}");
        }

        return errors;
    }

    /// <summary>
    /// Mapea un SteamGameDto a una entidad Juego.
    /// Realiza transformaciones y validaciones en el mapeo.
    /// </summary>
    private static Juego? MapSteamGameToJuego(SteamGameDto steamGame)
    {
        if (string.IsNullOrWhiteSpace(steamGame.Name))
            return null;

        var juego = new Juego
        {
            Id = Guid.NewGuid(),
            Nombre = steamGame.Name.Trim().Length > 200
                ? steamGame.Name.Trim()[..200]
                : steamGame.Name.Trim(),
            Descripcion = steamGame.Description?.Trim(),
            ImagenPortadaUrl = steamGame.HeaderImage,
            Plataforma = ExtractPlatforms(steamGame.Platforms)
        };

        // Procesar géneros
        if (!string.IsNullOrWhiteSpace(steamGame.Genres))
        {
            var genres = steamGame.Genres.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (genres.Length > 0)
            {
                juego.GeneroPrincipal = genres[0].Trim().Length > 100
                    ? genres[0].Trim()[..100]
                    : genres[0].Trim();

                juego.Tags = string.Join(", ", genres).Length > 500
                    ? string.Join(", ", genres)[..500]
                    : string.Join(", ", genres);
            }
        }

        // Procesar fecha de lanzamiento
        if (!string.IsNullOrWhiteSpace(steamGame.ReleaseDate))
        {
            if (DateOnly.TryParseExact(steamGame.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
            {
                juego.FechaLanzamiento = dateOnly;
            }
            else if (DateTime.TryParse(steamGame.ReleaseDate, out var dateTime))
            {
                juego.FechaLanzamiento = DateOnly.FromDateTime(dateTime);
            }
        }

        return juego;
    }

    /// <summary>
    /// Extrae las plataformas de un string JSON.
    /// Ejemplo: "Windows, Mac, Linux"
    /// </summary>
    private static string? ExtractPlatforms(string? platformsJson)
    {
        if (string.IsNullOrWhiteSpace(platformsJson))
            return null;

        try
        {
            var platforms = new List<string>();

            if (platformsJson.Contains("windows", StringComparison.OrdinalIgnoreCase))
                platforms.Add("Windows");
            if (platformsJson.Contains("mac", StringComparison.OrdinalIgnoreCase))
                platforms.Add("Mac");
            if (platformsJson.Contains("linux", StringComparison.OrdinalIgnoreCase))
                platforms.Add("Linux");

            return platforms.Count > 0 ? string.Join(", ", platforms) : null;
        }
        catch
        {
            return null;
        }
    }
}
