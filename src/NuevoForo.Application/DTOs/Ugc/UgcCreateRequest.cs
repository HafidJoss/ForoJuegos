using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NuevoForo.Application.DTOs.Ugc;

/// <summary>
/// DTO para crear contenido UGC con upload de archivo y foto.
/// El archivo se envía como multipart/form-data (no JSON).
/// 
/// Validaciones:
/// - Titulo: requerido, 2-200 caracteres
/// - Descripcion: opcional, máx 4000 caracteres
/// - JuegoId: opcional (backward compatibility)
/// - Tags: opcional, máx 1000 caracteres
/// - Archivo: requerido, máx 50 MB, sin restricciones de tipo
/// - Foto: opcional, máx 10 MB, sin restricciones de tipo
/// - DeclaracionLegalAceptada: debe ser true
/// </summary>
public sealed class UgcCreateRequest
{
    /// <summary>
    /// Título del contenido UGC.
    /// Ej: "Guía completa de Elden Ring", "Mod de texturas HD", "Parche balance"
    /// </summary>
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del contenido (opcional).
    /// Describe qué es el contenido, para qué sirve, compatibilidades, etc.
    /// </summary>
    [StringLength(4000, ErrorMessage = "La descripción no puede exceder 4000 caracteres")]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Identificador único del juego (opcional).
    /// Si se proporciona, se valida que el juego exista.
    /// Mantiene backward compatibility.
    /// </summary>
    public Guid? JuegoId { get; set; }

    /// <summary>
    /// Etiquetas para categorizar el contenido (ej: "mod, textura, HD").
    /// Ayuda a los usuarios a descubrir contenido similar.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Los tags no pueden exceder 1000 caracteres")]
    public string? Tags { get; set; }

    /// <summary>
    /// Archivo a subir. Se valida en el controlador.
    /// - Tamaño máximo: 50 MB
    /// - Soporta cualquier formato de archivo (SIN RESTRICCIONES)
    /// </summary>
    [Required(ErrorMessage = "El archivo es requerido")]
    public IFormFile? Archivo { get; set; }

    /// <summary>
    /// Foto/imagen del contenido UGC (opcional).
    /// Se almacena en carpeta separada: wwwroot/uploads/fotos/
    /// - Tamaño máximo: 10 MB
    /// - Soporta cualquier formato (SIN RESTRICCIONES)
    /// </summary>
    public IFormFile? Foto { get; set; }

    /// <summary>
    /// El usuario DEBE aceptar la declaración legal antes de publicar.
    /// Confirma que tiene derechos de distribución del contenido.
    /// </summary>
    [Required(ErrorMessage = "Debes aceptar la declaración legal")]
    public bool DeclaracionLegalAceptada { get; set; }
}
