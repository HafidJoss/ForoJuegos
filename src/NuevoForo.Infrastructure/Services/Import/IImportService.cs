using NuevoForo.Application.DTOs.Import;

namespace NuevoForo.Infrastructure.Services.Import;

/// <summary>
/// Interfaz para servicios de importación de datos de juegos.
/// Define operaciones para cargar datos de Steam u otras fuentes.
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Importa juegos desde un archivo JSON de Steam Web API Dump.
    /// El archivo debe contener un array de objetos con propiedades de juegos Steam.
    /// </summary>
    /// <param name="filePath">Ruta absoluta al archivo JSON de Steam games.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    /// <returns>ImportResult con estadísticas de la importación realizada.</returns>
    Task<ImportResult> ImportFromSteamDumpAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Descarga el archivo de Steam Web API Dump desde GitHub.
    /// Si already existe localmente, puede opcionalmente revalidarse.
    /// </summary>
    /// <param name="destinationPath">Ruta donde guardar el archivo descargado.</param>
    /// <param name="cancellationToken">Token para cancelar la descarga.</param>
    /// <returns>True si se descargó o ya existía; False si hubo error.</returns>
    Task<bool> DownloadSteamDumpAsync(string destinationPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida la estructura y contenido de un archivo JSON de Steam dump.
    /// Retorna una lista de errores encontrados.
    /// </summary>
    /// <param name="filePath">Ruta al archivo a validar.</param>
    /// <returns>Lista de errores encontrados (vacía si la validación es exitosa).</returns>
    Task<List<string>> ValidateSteamDumpAsync(string filePath);
}
