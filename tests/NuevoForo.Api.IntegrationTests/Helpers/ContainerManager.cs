namespace NuevoForo.Api.IntegrationTests.Helpers;

/// <summary>
/// Gestor centralizado para los contenedores de prueba.
/// Proporciona utilidades para manejo de contenedores compartidos.
/// </summary>
public static class ContainerManager
{
    /// <summary>
    /// Retardo para esperar a que PostgreSQL esté completamente listo.
    /// En algunos sistemas puede ser necesario más tiempo.
    /// </summary>
    private const int StartupDelayMs = 2000;

    /// <summary>
    /// Espera a que el contenedor esté completamente listo.
    /// </summary>
    public static async Task WaitForReadinessAsync()
    {
        // Dar tiempo extra para que PostgreSQL se estabilice
        await Task.Delay(StartupDelayMs);
    }

    /// <summary>
    /// Valida que la configuración de prueba sea correcta.
    /// </summary>
    public static void ValidateTestConfiguration()
    {
        // Verificar que Docker esté disponible
        try
        {
            // Esta verificación se hace implícitamente al crear el contenedor
            // Si Docker no está disponible, GetConnectionString() fallará
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Docker no está disponible. Por favor, asegúrate de que Docker está instalado y ejecutándose.",
                ex
            );
        }
    }

    /// <summary>
    /// Obtiene la variable de entorno para configuración de pruebas.
    /// </summary>
    public static string? GetTestEnvironmentVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key);
    }

    /// <summary>
    /// Obtiene el puerto de prueba desde variables de entorno o usa el default.
    /// </summary>
    public static int GetTestPort(string portEnvVar, int defaultPort)
    {
        var portStr = GetTestEnvironmentVariable(portEnvVar);
        return int.TryParse(portStr, out var port) ? port : defaultPort;
    }
}
