using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Ugc;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

/// <summary>
/// Servicio para gestionar contenido UGC (User Generated Content).
/// Maneja la creación, actualización, eliminación y recuperación de contenido.
/// Incluye la lógica para almacenar archivos localmente y generar URLs accesibles.
/// </summary>
public class UgcService(AppDbContext dbContext, IWebHostEnvironment environment) : IUgcService
{
    // Configuración de almacenamiento
    private const string UploadsFolder = "uploads";
    private const string UgcSubFolder = "ugc";
    private const string FotosSubFolder = "fotos";

    public async Task<UgcResponse> CreateAsync(Guid usuarioId, UgcCreateRequest request, CancellationToken cancellationToken = default)
    {
        // =============== VALIDACIONES ===============
        if (!request.DeclaracionLegalAceptada)
        {
            throw new InvalidOperationException("Debe aceptar la declaración legal para publicar contenido.");
        }

        if (request.Archivo is null || request.Archivo.Length == 0)
        {
            throw new ArgumentException("El archivo es requerido.");
        }

        // =============== PREPARACIÓN DE DIRECTORIOS ===============
        var uploadsPath = GetUploadsPath();
        var fotosPath = Path.Combine(Path.GetDirectoryName(uploadsPath)!, FotosSubFolder);

        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        if (!Directory.Exists(fotosPath))
        {
            Directory.CreateDirectory(fotosPath);
        }

        // =============== PROCESAR ARCHIVO PRINCIPAL ===============
        var fileId = Guid.NewGuid();
        var originalFileName = Path.GetFileName(request.Archivo.FileName);
        var uniqueFileName = $"{fileId}_{originalFileName}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        // Guardar archivo en disco
        await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await request.Archivo.CopyToAsync(stream, cancellationToken);
        }

        // Calcular hash SHA256 del archivo
        string archivoHash;
        await using (var stream = System.IO.File.OpenRead(filePath))
        {
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
            archivoHash = Convert.ToBase64String(hashBytes);
        }

        var archivoUrl = $"/uploads/{UgcSubFolder}/{uniqueFileName}";

        // =============== PROCESAR FOTO (OPCIONAL) ===============
        string? fotoPath = null;
        string? fotoNombre = null;
        long? fotoSize = null;

        if (request.Foto is not null && request.Foto.Length > 0)
        {
            var fotoId = Guid.NewGuid();
            var originalFotoName = Path.GetFileName(request.Foto.FileName);
            var uniqueFotoName = $"{fotoId}_{originalFotoName}";
            var fotoFilePath = Path.Combine(fotosPath, uniqueFotoName);

            // Guardar foto en disco
            await using (var stream = new FileStream(fotoFilePath, FileMode.Create, FileAccess.Write))
            {
                await request.Foto.CopyToAsync(stream, cancellationToken);
            }

            fotoPath = $"/uploads/{FotosSubFolder}/{uniqueFotoName}";
            fotoNombre = originalFotoName;
            fotoSize = request.Foto.Length;
        }

        // =============== CREAR ENTIDAD EN BD ===============
        var ugc = new ContenidoUgc
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            JuegoId = request.JuegoId,
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            Tags = request.Tags?.Trim(),
            ArchivoUrl = archivoUrl,
            ArchivoNombre = originalFileName,
            ArchivoSize = request.Archivo.Length,
            ArchivoHash = archivoHash,
            FotoPath = fotoPath,
            FotoNombre = fotoNombre,
            FotoSize = fotoSize,
            DeclaracionLegalAceptada = true,
            FechaSubida = DateTime.UtcNow,
            Estado = EstadoContenidoUgc.Publicado
        };

        // =============== PERSISTIR EN BD ===============
        dbContext.ContenidosUgc.Add(ugc);
        await dbContext.SaveChangesAsync(cancellationToken);

        string? juegoNombre = null;
        if (ugc.JuegoId.HasValue)
        {
            var juego = await dbContext.Juegos.FindAsync(new object[] { ugc.JuegoId.Value }, cancellationToken);
            juegoNombre = juego?.Nombre;
        }

        return MapToResponse(ugc, juegoNombre);
    }

    public async Task<UgcResponse?> UpdateAsync(Guid id, Guid usuarioId, bool esModerador, UgcUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var ugc = await dbContext.ContenidosUgc.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (ugc is null)
        {
            return null;
        }

        // Solo el propietario o moderadores pueden actualizar
        if (!esModerador && ugc.UsuarioId != usuarioId)
        {
            return null;
        }

        ugc.Titulo = request.Titulo.Trim();
        ugc.Descripcion = request.Descripcion?.Trim();
        ugc.Tags = request.Tags?.Trim();
        ugc.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(ugc);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default)
    {
        var ugc = await dbContext.ContenidosUgc.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (ugc is null)
        {
            return false;
        }

        // Solo el propietario o moderadores pueden eliminar
        if (!esModerador && ugc.UsuarioId != usuarioId)
        {
            return false;
        }

        // Marcar como eliminado (soft delete) en lugar de borrar
        ugc.Estado = EstadoContenidoUgc.Eliminado;
        ugc.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        // Opcionalmente, eliminar archivo del disco (descomenta si lo necesitas)
        // DeleteFileFromDisk(ugc.ArchivoUrl);

        return true;
    }

    public async Task<UgcResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ugc = await dbContext.ContenidosUgc
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return ugc is null ? null : MapToResponse(ugc);
    }

    public async Task<IReadOnlyList<UgcResponse>> GetByGameAsync(Guid juegoId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        // Validar paginación
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var ugcs = await dbContext.ContenidosUgc
            .AsNoTracking()
            .Where(c => c.JuegoId == juegoId && c.Estado == EstadoContenidoUgc.Publicado)
            .OrderByDescending(c => c.FechaSubida)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new UgcResponse
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                JuegoId = c.JuegoId,
                JuegoNombre = c.Juego != null ? c.Juego.Nombre : null,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                ArchivoUrl = c.ArchivoUrl,
                ArchivoNombre = c.ArchivoNombre,
                ArchivoSize = c.ArchivoSize,
                FotoPath = c.FotoPath,
                FotoNombre = c.FotoNombre,
                FotoSize = c.FotoSize,
                Tags = c.Tags,
                DeclaracionLegalAceptada = c.DeclaracionLegalAceptada,
                FechaSubida = c.FechaSubida,
                FechaActualizacion = c.FechaActualizacion,
                Estado = c.Estado.ToString()
            })
            .ToListAsync(cancellationToken);

        return ugcs;
    }

    public async Task<IReadOnlyList<UgcResponse>> GetByUserAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var ugcs = await dbContext.ContenidosUgc
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId && c.Estado == EstadoContenidoUgc.Publicado)
            .OrderByDescending(c => c.FechaSubida)
            .Select(c => new UgcResponse
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                JuegoId = c.JuegoId,
                JuegoNombre = c.Juego != null ? c.Juego.Nombre : null,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                ArchivoUrl = c.ArchivoUrl,
                ArchivoNombre = c.ArchivoNombre,
                ArchivoSize = c.ArchivoSize,
                FotoPath = c.FotoPath,
                FotoNombre = c.FotoNombre,
                FotoSize = c.FotoSize,
                Tags = c.Tags,
                DeclaracionLegalAceptada = c.DeclaracionLegalAceptada,
                FechaSubida = c.FechaSubida,
                FechaActualizacion = c.FechaActualizacion,
                Estado = c.Estado.ToString()
            })
            .ToListAsync(cancellationToken);

        return ugcs;
    }

    public async Task<IReadOnlyList<UgcResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var ugcs = await dbContext.ContenidosUgc
            .AsNoTracking()
            .Where(c => c.Estado == EstadoContenidoUgc.Publicado)
            .OrderByDescending(c => c.FechaSubida)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new UgcResponse
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                JuegoId = c.JuegoId,
                JuegoNombre = c.Juego != null ? c.Juego.Nombre : null,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                ArchivoUrl = c.ArchivoUrl,
                ArchivoNombre = c.ArchivoNombre,
                ArchivoSize = c.ArchivoSize,
                FotoPath = c.FotoPath,
                FotoNombre = c.FotoNombre,
                FotoSize = c.FotoSize,
                Tags = c.Tags,
                DeclaracionLegalAceptada = c.DeclaracionLegalAceptada,
                FechaSubida = c.FechaSubida,
                FechaActualizacion = c.FechaActualizacion,
                Estado = c.Estado.ToString()
            })
            .ToListAsync(cancellationToken);

        return ugcs;
    }

    /// <summary>
    /// Obtiene la ruta completa de la carpeta de uploads en el servidor.
    /// </summary>
    private string GetUploadsPath()
    {
        // Si WebRootPath es nulo, usar ContentRootPath + "wwwroot"
        string webRootPath = environment.WebRootPath ?? 
            Path.Combine(environment.ContentRootPath, "wwwroot");

        var uploadsPath = Path.Combine(webRootPath, UploadsFolder, UgcSubFolder);
        return uploadsPath;
    }

    /// <summary>
    /// Convierte una entidad ContenidoUgc a su DTO de respuesta.
    /// </summary>
    private static UgcResponse MapToResponse(ContenidoUgc ugc, string? juegoNombre = null)
    {
        return new UgcResponse
        {
            Id = ugc.Id,
            UsuarioId = ugc.UsuarioId,
            JuegoId = ugc.JuegoId,
            JuegoNombre = juegoNombre ?? ugc.Juego?.Nombre,
            Titulo = ugc.Titulo,
            Descripcion = ugc.Descripcion,
            ArchivoUrl = ugc.ArchivoUrl,
            ArchivoNombre = ugc.ArchivoNombre,
            ArchivoSize = ugc.ArchivoSize,
            FotoPath = ugc.FotoPath,
            FotoNombre = ugc.FotoNombre,
            FotoSize = ugc.FotoSize,
            Tags = ugc.Tags,
            DeclaracionLegalAceptada = ugc.DeclaracionLegalAceptada,
            FechaSubida = ugc.FechaSubida,
            FechaActualizacion = ugc.FechaActualizacion,
            Estado = ugc.Estado.ToString()
        };
    }

    /// <summary>
    /// Elimina un archivo del disco (opcional, se puede usar en eliminación permanente).
    /// </summary>
    private void DeleteFileFromDisk(string archivoUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(archivoUrl))
                return;

            // Extrae el nombre del archivo de la URL
            var fileName = Path.GetFileName(archivoUrl);
            var filePath = Path.Combine(GetUploadsPath(), fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch
        {
            // Log del error pero no lanzar excepción
            // Logger.LogWarning("Error al eliminar archivo: {FilePath}", archivoUrl);
        }
    }
}
