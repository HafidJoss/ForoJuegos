using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Reviews;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class ReviewService(AppDbContext dbContext) : IReviewService
{
    public async Task<ReviewResponse> CreateAsync(Guid usuarioId, ReviewCreateRequest request, CancellationToken cancellationToken = default)
    {
        var resena = new Resena
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            JuegoId = request.JuegoId,
            Texto = request.Texto.Trim(),
            Rating = request.Rating,
            Estado = EstadoResena.Activa,
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.Resenas.Add(resena);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapToResponseAsync(resena, cancellationToken);
    }

    public async Task<ReviewResponse?> UpdateAsync(Guid id, Guid usuarioId, bool esModerador, ReviewUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var resena = await dbContext.Resenas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resena is null)
        {
            return null;
        }

        if (!esModerador && resena.UsuarioId != usuarioId)
        {
            return null;
        }

        resena.Texto = request.Texto.Trim();
        resena.Rating = request.Rating;
        resena.FechaActualizacion = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await MapToResponseAsync(resena, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default)
    {
        var resena = await dbContext.Resenas.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (resena is null)
        {
            return false;
        }

        if (!esModerador && resena.UsuarioId != usuarioId)
        {
            return false;
        }

        resena.Estado = EstadoResena.Eliminada;
        resena.FechaActualizacion = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetByGameAsync(Guid juegoId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var resenas = await dbContext.Resenas
            .AsNoTracking()
            .Where(r => r.JuegoId == juegoId && r.Estado == EstadoResena.Activa)
            .OrderByDescending(r => r.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                JuegoId = r.JuegoId,
                JuegoNombre = r.Juego.Nombre,
                Texto = r.Texto,
                Rating = r.Rating,
                FechaCreacion = r.FechaCreacion,
                FechaActualizacion = r.FechaActualizacion,
                Likes = r.Likes.Count
            })
            .ToListAsync(cancellationToken);

        return resenas;
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetByUserAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var resenas = await dbContext.Resenas
            .AsNoTracking()
            .Where(r => r.UsuarioId == usuarioId && r.Estado == EstadoResena.Activa)
            .OrderByDescending(r => r.FechaCreacion)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                UsuarioId = r.UsuarioId,
                JuegoId = r.JuegoId,
                JuegoNombre = r.Juego.Nombre,
                Texto = r.Texto,
                Rating = r.Rating,
                FechaCreacion = r.FechaCreacion,
                FechaActualizacion = r.FechaActualizacion,
                Likes = r.Likes.Count
            })
            .ToListAsync(cancellationToken);

        return resenas;
    }

    private async Task<ReviewResponse> MapToResponseAsync(Resena resena, CancellationToken cancellationToken)
    {
        var likes = await dbContext.LikesResena.CountAsync(l => l.ResenaId == resena.Id, cancellationToken);
        
        var juegoNombre = resena.Juego?.Nombre;
        if (string.IsNullOrEmpty(juegoNombre))
        {
            var juego = await dbContext.Juegos.FindAsync(new object[] { resena.JuegoId }, cancellationToken);
            juegoNombre = juego?.Nombre;
        }

        return new ReviewResponse
        {
            Id = resena.Id,
            UsuarioId = resena.UsuarioId,
            JuegoId = resena.JuegoId,
            JuegoNombre = juegoNombre,
            Texto = resena.Texto,
            Rating = resena.Rating,
            FechaCreacion = resena.FechaCreacion,
            FechaActualizacion = resena.FechaActualizacion,
            Likes = likes
        };
    }
}
