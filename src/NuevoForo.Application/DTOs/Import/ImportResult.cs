namespace NuevoForo.Application.DTOs.Import;

/// <summary>
/// Resultado de una operación de importación de juegos.
/// Contiene estadísticas y detalles del proceso de importación.
/// </summary>
public sealed class ImportResult
{
    /// <summary>
    /// Total de registros procesados (leídos del archivo).
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Total de juegos insertados exitosamente en la BD.
    /// </summary>
    public int Inserted { get; set; }

    /// <summary>
    /// Total de registros duplicados (ya existen en BD).
    /// No se insertan para evitar violaciones de constraints.
    /// </summary>
    public int Duplicates { get; set; }

    /// <summary>
    /// Total de registros que fallaron al procesar.
    /// Pueden ser por validación fallida o errores de BD.
    /// </summary>
    public int Failed { get; set; }

    /// <summary>
    /// Lista de mensajes de error detallados.
    /// Cada error indica qué falló y en qué registro.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Fecha/hora de inicio de la importación.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Fecha/hora de finalización de la importación.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Duración total de la importación en milisegundos.
    /// </summary>
    public long DurationMs => (long)(EndTime - StartTime).TotalMilliseconds;

    /// <summary>
    /// Indica si la importación fue exitosa (sin errores críticos).
    /// True si Inserted > 0 y Failed == 0.
    /// </summary>
    public bool IsSuccess => Failed == 0 && Inserted > 0;

    /// <summary>
    /// Porcentaje de éxito de la importación (0-100).
    /// Cálculo: (Inserted / Total) * 100
    /// </summary>
    public double SuccessPercentage => Total > 0 ? (Inserted / (double)Total) * 100 : 0;

    /// <summary>
    /// Resumen legible del resultado de la importación.
    /// Incluye estadísticas principales.
    /// </summary>
    public string Summary => $"Importación completada en {DurationMs}ms: " +
        $"{Inserted} insertados, {Duplicates} duplicados, {Failed} fallos (Éxito: {SuccessPercentage:F1}%)";

    /// <summary>
    /// Agrega un mensaje de error a la lista.
    /// </summary>
    public void AddError(string error)
    {
        Errors.Add(error);
        Failed++;
    }

    /// <summary>
    /// Agrega múltiples mensajes de error.
    /// </summary>
    public void AddErrors(params string[] errors)
    {
        foreach (var error in errors)
        {
            AddError(error);
        }
    }
}
