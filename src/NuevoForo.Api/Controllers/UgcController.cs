using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Ugc;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

/// <summary>
/// Controlador para gestionar contenido UGC (User Generated Content).
/// Permite que usuarios autorizados suban archivos (guías, mods, parches, etc.)
/// para juegos específicos dentro de la plataforma.
/// </summary>
[ApiController]
[Route("ugc")]
public class UgcController(IUgcService ugcService, IGameService gameService, ILikeService likeService, IWebHostEnvironment environment, ILogger<UgcController> logger) : ControllerBase
{
    /// <summary>
    /// Crea y sube nuevo contenido UGC con archivo y foto opcionales.
    /// 
    /// Enviar como multipart/form-data con los siguientes campos:
    /// - titulo (string, requerido): 2-200 caracteres
    /// - descripcion (string, opcional): máx 4000 caracteres
    /// - juegoId (Guid, opcional): Si se proporciona, debe existir en BD
    /// - tags (string, opcional): máx 1000 caracteres
    /// - archivo (file, requerido): máx 50 MB, SIN restricciones de tipo
    /// - foto (file, opcional): máx 10 MB, SIN restricciones de tipo
    /// - declaracionLegalAceptada (boolean, requerido): debe ser true
    /// 
    /// Validaciones:
    /// - Usuario debe estar autenticado (Bearer token)
    /// - Archivo es obligatorio
    /// - JuegoId es opcional (backward compatibility)
    /// - Foto es opcional (para preview/thumbnail)
    /// - Usuario debe aceptar declaración legal
    /// 
    /// Respuesta:
    /// - 201 Created: UGC creado exitosamente
    /// - 400 Bad Request: Validación fallida
    /// - 404 Not Found: Juego no existe (si se proporciona JuegoId)
    /// - 401 Unauthorized: Usuario no autenticado
    /// - 500 Internal Server Error: Error al procesar archivos
    /// 
    /// Almacenamiento:
    /// - Archivo: wwwroot/uploads/ugc/{guid}_{nombreOriginal}
    /// - Foto: wwwroot/uploads/fotos/{guid}_{nombreOriginal}
    /// - Ambos accesibles vía HTTP: /uploads/ugc/... y /uploads/fotos/...
    /// </summary>
    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<ActionResult<UgcResponse>> Create([FromForm] UgcCreateRequest request, CancellationToken cancellationToken)
    {
        // Validación del modelo
        if (!ModelState.IsValid)
        {
            logger.LogWarning("ModelState inválido en UGC Create: {@ModelState}", ModelState);
            return ValidationProblem(ModelState);
        }

        // Validación de declaración legal
        if (!request.DeclaracionLegalAceptada)
        {
            logger.LogWarning("Usuario {UserId} intentó publicar sin aceptar declaración legal", GetUserId());
            return BadRequest(new { message = "Debe aceptar la declaración legal para publicar contenido." });
        }

        // Validación de archivo
        if (request.Archivo is null || request.Archivo.Length == 0)
        {
            logger.LogWarning("Archivo vacío en UGC Create");
            return BadRequest(new { message = "Debe seleccionar un archivo para subir." });
        }

        // Validación de JuegoId (opcional)
        if (request.JuegoId.HasValue && request.JuegoId != Guid.Empty)
        {
            // Verificar que el juego existe si se proporciona
            var juego = await gameService.GetByIdAsync(request.JuegoId.Value, cancellationToken);
            if (juego is null)
            {
                logger.LogWarning("Juego {JuegoId} no encontrado en UGC Create", request.JuegoId);
                return NotFound(new { message = $"El juego con ID {request.JuegoId} no existe." });
            }
        }

        // Obtener ID del usuario autenticado
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            logger.LogWarning("No se pudo obtener el ID del usuario");
            return Unauthorized();
        }

        try
        {
            logger.LogInformation("Iniciando creación de UGC para usuario {UserId}, juego: {JuegoId}, archivo: {FileName}", usuarioId, request.JuegoId, request.Archivo.FileName);
            var created = await ugcService.CreateAsync(usuarioId.Value, request, cancellationToken);
            logger.LogInformation("UGC creado exitosamente con ID: {UgcId}", created.Id);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear UGC para usuario {UserId}", usuarioId);
            return StatusCode(500, new { message = "Error al procesar el archivo. Intenta de nuevo." });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UgcResponse>> Update(Guid id, UgcUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var esModerador = User.IsInRole("Admin") || User.IsInRole("Moderador");
        var updated = await ugcService.UpdateAsync(id, usuarioId.Value, esModerador, request, cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var esModerador = User.IsInRole("Admin") || User.IsInRole("Moderador");
        var deleted = await ugcService.DeleteAsync(id, usuarioId.Value, esModerador, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<UgcResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var ugc = await ugcService.GetByIdAsync(id, cancellationToken);
        return ugc is null ? NotFound() : Ok(ugc);
    }

    [HttpGet("by-game/{juegoId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<UgcResponse>>> GetByGame(Guid juegoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var ugcs = await ugcService.GetByGameAsync(juegoId, page, pageSize, cancellationToken);
        return Ok(ugcs);
    }

    [HttpGet("by-user/{usuarioId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<UgcResponse>>> GetByUser(Guid usuarioId, CancellationToken cancellationToken)
    {
        var ugcs = await ugcService.GetByUserAsync(usuarioId, cancellationToken);
        return Ok(ugcs);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<UgcResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var ugcs = await ugcService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(ugcs);
    }

    /// <summary>
    /// Endpoint para forzar la descarga de un archivo UGC de forma directa y nativa sin problemas de CORS.
    /// </summary>
    [HttpGet("download/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var ugc = await ugcService.GetByIdAsync(id, cancellationToken);
        if (ugc is null || string.IsNullOrEmpty(ugc.ArchivoUrl))
        {
            logger.LogWarning("Intento de descargar UGC {UgcId} inválido o sin archivo", id);
            return NotFound(new { message = "Archivo no encontrado." });
        }

        // Obtener ruta completa en el servidor
        string webRootPath = environment.WebRootPath ?? 
            Path.Combine(environment.ContentRootPath, "wwwroot");
            
        var filePath = Path.Combine(webRootPath, ugc.ArchivoUrl.TrimStart('/'));

        if (!System.IO.File.Exists(filePath))
        {
            logger.LogWarning("Archivo físico no encontrado para UGC {UgcId} en ruta {Path}", id, filePath);
            return NotFound(new { message = "El archivo físico no existe en el servidor." });
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
        logger.LogInformation("Descargando archivo {FileName} de UGC {UgcId}", ugc.ArchivoNombre, id);
        return File(fileBytes, "application/octet-stream", ugc.ArchivoNombre);
    }

    [HttpPost("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> Like(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var added = await likeService.AddUgcLikeAsync(id, usuarioId.Value, cancellationToken);
        return added ? NoContent() : Conflict(new { message = "Ya has dado like a este contenido." });
    }

    [HttpDelete("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> Unlike(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var removed = await likeService.RemoveUgcLikeAsync(id, usuarioId.Value, cancellationToken);
        return removed ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/dislike")]
    [Authorize]
    public async Task<IActionResult> Dislike(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var added = await likeService.AddUgcDislikeAsync(id, usuarioId.Value, cancellationToken);
        return added ? NoContent() : Conflict(new { message = "Ya has dado dislike a este contenido." });
    }

    [HttpGet("{id:guid}/likes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLikeCounts(Guid id, CancellationToken cancellationToken)
    {
        var counts = await likeService.GetUgcLikeCountsAsync(id, cancellationToken);
        return Ok(counts);
    }

    /// <summary>
    /// Obtiene el ID del usuario autenticado desde los claims de seguridad.
    /// </summary>
    private Guid? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
