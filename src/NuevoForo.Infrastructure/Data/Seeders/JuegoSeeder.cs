using Microsoft.Extensions.Logging;
using NuevoForo.Application.DTOs.Import;
using NuevoForo.Infrastructure.Services.Import;

namespace NuevoForo.Infrastructure.Data.Seeders;

/// <summary>
/// Seeder para poblar la tabla de Juegos desde Steam Web API Dump.
/// Se ejecuta durante la inicialización de la aplicación si la tabla está vacía.
/// </summary>
public sealed class JuegoSeeder
{
    private readonly IImportService _importService;
    private readonly ILogger<JuegoSeeder> _logger;

    // Rutas de datos
    private const string SteamDumpFileName = "steam_games.json";

    public JuegoSeeder(IImportService importService, ILogger<JuegoSeeder> logger)
    {
        _importService = importService ?? throw new ArgumentNullException(nameof(importService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Ejecuta el seeding de juegos desde Steam dump.
    /// Primero busca un archivo local en la carpeta de datos; si no existe, intenta descargarlo.
    /// </summary>
    /// <param name="dataDirectory">Directorio donde almacenar datos de importación.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>ImportResult con estadísticas del seeding.</returns>
    public async Task<ImportResult> SeedJuegosFromSteamAsync(
        string dataDirectory,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando seeding de Juegos desde Steam dataset");

        try
        {
            // Preparar ruta del archivo
            var steamDumpPath = Path.Combine(dataDirectory, SteamDumpFileName);

            _logger.LogInformation("Ruta esperada de Steam dump: {Path}", steamDumpPath);

            // Crear directorio si no existe
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                _logger.LogInformation("Directorio de datos creado: {Directory}", dataDirectory);
            }

            // Buscar archivo local - primero en la carpeta actual, luego en raíz del proyecto
            var localSteamFiles = new[]
            {
                steamDumpPath,
                Path.Combine(Directory.GetCurrentDirectory(), "data", SteamDumpFileName),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "data", SteamDumpFileName),
                Path.Combine(AppContext.BaseDirectory, "data", SteamDumpFileName)
            };

            string? existingFile = null;
            foreach (var file in localSteamFiles)
            {
                if (File.Exists(file))
                {
                    _logger.LogInformation("✅ Archivo Steam dump encontrado localmente: {Path}", file);
                    existingFile = file;
                    break;
                }
            }

            // Si no existe archivo local, intentar descargar
            if (existingFile == null)
            {
                _logger.LogInformation("Archivo local no encontrado. Intentando descargar desde GitHub...");

                var downloaded = await _importService.DownloadSteamDumpAsync(
                    steamDumpPath,
                    cancellationToken);

                if (!downloaded)
                {
                    _logger.LogWarning("⚠️ No se pudo descargar desde GitHub. Verificando alternativas...");

                    // Buscar nuevamente por si se descargó en otra ruta
                    existingFile = localSteamFiles.FirstOrDefault(File.Exists);

                    if (existingFile == null)
                    {
                        var errorMsg = "No se pudo encontrar o descargar el archivo de Steam dump";
                        _logger.LogError(errorMsg);

                        var result = new ImportResult
                        {
                            StartTime = DateTime.UtcNow,
                            EndTime = DateTime.UtcNow
                        };
                        result.AddError(errorMsg);
                        return result;
                    }
                }
                else
                {
                    existingFile = steamDumpPath;
                    _logger.LogInformation("✅ Steam dump descargado exitosamente");
                }
            }

            // Validar estructura del archivo
            _logger.LogInformation("Validando estructura del archivo...");
            var validationErrors = await _importService.ValidateSteamDumpAsync(existingFile);

            if (validationErrors.Count > 0)
            {
                _logger.LogWarning("Errores de validación encontrados: {Count}", validationErrors.Count);
                foreach (var error in validationErrors.Take(5))
                {
                    _logger.LogWarning("  - {Error}", error);
                }
            }
            else
            {
                _logger.LogInformation("✅ Validación exitosa");
            }

            // Realizar importación
            _logger.LogInformation("Comenzando importación de juegos desde: {FilePath}", existingFile);
            var importResult = await _importService.ImportFromSteamDumpAsync(
                existingFile,
                cancellationToken);

            _logger.LogInformation("Importación completada: {Summary}", importResult.Summary);

            return importResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error durante el seeding de juegos");

            var result = new ImportResult
            {
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow
            };
            result.AddError($"Seeding abortado: {ex.Message}");

            return result;
        }
    }
}
