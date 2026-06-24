namespace NuevoForo.Api.IntegrationTests.Helpers;

/// <summary>
/// Extensiones para assertions personalizadas en pruebas de integración.
/// </summary>
public static class AssertionExtensions
{
    /// <summary>
    /// Verifica que una colección no sea nula y no esté vacía.
    /// </summary>
    public static void ShouldNotBeNullOrEmpty<T>(this IEnumerable<T>? collection, string? message = null)
    {
        if (collection == null || !collection.Any())
        {
            throw new AssertionFailedException(
                message ?? "La colección no debería ser nula o estar vacía"
            );
        }
    }

    /// <summary>
    /// Verifica que una colección esté vacía.
    /// </summary>
    public static void ShouldBeEmpty<T>(this IEnumerable<T> collection, string? message = null)
    {
        if (collection.Any())
        {
            throw new AssertionFailedException(
                message ?? "La colección debería estar vacía"
            );
        }
    }

    /// <summary>
    /// Verifica que el count de una colección sea igual al esperado.
    /// </summary>
    public static void ShouldHaveCount<T>(this IEnumerable<T> collection, int expectedCount, string? message = null)
    {
        var actualCount = collection.Count();
        if (actualCount != expectedCount)
        {
            throw new AssertionFailedException(
                message ?? $"Se esperaban {expectedCount} elementos, pero se encontraron {actualCount}"
            );
        }
    }
}

/// <summary>
/// Excepción personalizada para assertions fallidas.
/// </summary>
public class AssertionFailedException : Exception
{
    public AssertionFailedException(string message) : base(message) { }
}
