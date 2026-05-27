using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Reports;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class ReportService(AppDbContext dbContext) : IReportService
{
    public async Task<ReportResponse> CreateAsync(Guid usuarioId, ReportCreateRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateTargetAsync(request.TipoObjetivo, request.ObjetivoId, cancellationToken);

        var reporte = new Reporte
        {
            Id = Guid.NewGuid(),
            ReportadoPorUsuarioId = usuarioId,
            TipoObjetivo = request.TipoObjetivo,
            ObjetivoId = request.ObjetivoId,
            Motivo = request.Motivo.Trim(),
            Evidencia = request.Evidencia?.Trim(),
            Estado = EstadoReporte.Abierto,
            FechaCreacion = DateTime.UtcNow,
            AccionTomada = AccionModeracion.Ninguna
        };

        dbContext.Reportes.Add(reporte);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(reporte);
    }

    public async Task<IReadOnlyList<ReportResponse>> ListAsync(EstadoReporte? estado, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var query = dbContext.Reportes.AsNoTracking();
        if (estado.HasValue)
        {
            query = query.Where(r => r.Estado == estado);
        }

        var reportes = await query
            .OrderByDescending(r => r.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReportResponse
            {
                Id = r.Id,
                ReportadoPorUsuarioId = r.ReportadoPorUsuarioId,
                TipoObjetivo = r.TipoObjetivo,
                ObjetivoId = r.ObjetivoId,
                Motivo = r.Motivo,
                Evidencia = r.Evidencia,
                Estado = r.Estado,
                FechaCreacion = r.FechaCreacion,
                FechaCierre = r.FechaCierre,
                ModeradorId = r.ModeradorId,
                AccionTomada = r.AccionTomada
            })
            .ToListAsync(cancellationToken);

        return reportes;
    }

    public async Task<ReportResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reporte = await dbContext.Reportes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return reporte is null ? null : MapToResponse(reporte);
    }

    public async Task<ReportResponse?> ModerateAsync(Guid id, Guid moderadorId, AccionModeracion accion, bool rechazar, CancellationToken cancellationToken = default)
    {
        var reporte = await dbContext.Reportes.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (reporte is null)
        {
            return null;
        }

        if (reporte.Estado is EstadoReporte.Resuelto or EstadoReporte.Rechazado)
        {
            return null;
        }

        if (!rechazar)
        {
            await ApplyActionAsync(reporte.TipoObjetivo, reporte.ObjetivoId, accion, cancellationToken);
            reporte.AccionTomada = accion;
            reporte.Estado = EstadoReporte.Resuelto;
        }
        else
        {
            reporte.AccionTomada = AccionModeracion.Ninguna;
            reporte.Estado = EstadoReporte.Rechazado;
        }

        reporte.ModeradorId = moderadorId;
        reporte.FechaCierre = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(reporte);
    }

    private async Task ValidateTargetAsync(TipoObjetivoReporte tipoObjetivo, Guid objetivoId, CancellationToken cancellationToken)
    {
        var exists = tipoObjetivo switch
        {
            TipoObjetivoReporte.Usuario => await dbContext.Usuarios.AnyAsync(u => u.Id == objetivoId, cancellationToken),
            TipoObjetivoReporte.Resena => await dbContext.Resenas.AnyAsync(r => r.Id == objetivoId, cancellationToken),
            TipoObjetivoReporte.Comentario => await dbContext.Comentarios.AnyAsync(c => c.Id == objetivoId, cancellationToken),
            TipoObjetivoReporte.ContenidoUgc => await dbContext.ContenidosUgc.AnyAsync(c => c.Id == objetivoId, cancellationToken),
            _ => false
        };

        if (!exists)
        {
            throw new InvalidOperationException("Objetivo del reporte no encontrado.");
        }
    }

    private async Task ApplyActionAsync(TipoObjetivoReporte tipoObjetivo, Guid objetivoId, AccionModeracion accion, CancellationToken cancellationToken)
    {
        switch (tipoObjetivo)
        {
            case TipoObjetivoReporte.Usuario:
            {
                var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Id == objetivoId, cancellationToken);
                if (usuario is null)
                {
                    return;
                }

                if (accion == AccionModeracion.Suspender)
                {
                    usuario.Estado = EstadoUsuario.Suspendido;
                }

                break;
            }
            case TipoObjetivoReporte.Resena:
            {
                var resena = await dbContext.Resenas.FirstOrDefaultAsync(r => r.Id == objetivoId, cancellationToken);
                if (resena is null)
                {
                    return;
                }

                if (accion == AccionModeracion.Ocultar || accion == AccionModeracion.Eliminar)
                {
                    resena.Estado = EstadoResena.Eliminada;
                }

                break;
            }
            case TipoObjetivoReporte.Comentario:
            {
                var comentario = await dbContext.Comentarios.FirstOrDefaultAsync(c => c.Id == objetivoId, cancellationToken);
                if (comentario is null)
                {
                    return;
                }

                if (accion == AccionModeracion.Ocultar || accion == AccionModeracion.Eliminar)
                {
                    comentario.Estado = EstadoComentario.Eliminado;
                }

                break;
            }
            case TipoObjetivoReporte.ContenidoUgc:
            {
                var ugc = await dbContext.ContenidosUgc.FirstOrDefaultAsync(c => c.Id == objetivoId, cancellationToken);
                if (ugc is null)
                {
                    return;
                }

                if (accion == AccionModeracion.Ocultar || accion == AccionModeracion.Eliminar)
                {
                    ugc.Estado = EstadoContenidoUgc.Oculto;
                    if (accion == AccionModeracion.Eliminar)
                    {
                        ugc.Estado = EstadoContenidoUgc.Eliminado;
                    }
                }

                break;
            }
        }
    }

    private static ReportResponse MapToResponse(Reporte reporte)
    {
        return new ReportResponse
        {
            Id = reporte.Id,
            ReportadoPorUsuarioId = reporte.ReportadoPorUsuarioId,
            TipoObjetivo = reporte.TipoObjetivo,
            ObjetivoId = reporte.ObjetivoId,
            Motivo = reporte.Motivo,
            Evidencia = reporte.Evidencia,
            Estado = reporte.Estado,
            FechaCreacion = reporte.FechaCreacion,
            FechaCierre = reporte.FechaCierre,
            ModeradorId = reporte.ModeradorId,
            AccionTomada = reporte.AccionTomada
        };
    }
}
