using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Likes;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class LikeService(AppDbContext dbContext) : ILikeService
{
    public async Task<bool> AddLikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.LikesResena
            .FirstOrDefaultAsync(l => l.ResenaId == resenaId && l.UsuarioId == usuarioId, cancellationToken);

        if (existing is not null)
        {
            if (!existing.EsDislike)
            {
                // Ya tiene like
                return false;
            }

            // Cambiar dislike a like
            existing.EsDislike = false;
            existing.FechaCreacion = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        var like = new LikeResena
        {
            Id = Guid.NewGuid(),
            ResenaId = resenaId,
            UsuarioId = usuarioId,
            EsDislike = false,
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.LikesResena.Add(like);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AddDislikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.LikesResena
            .FirstOrDefaultAsync(l => l.ResenaId == resenaId && l.UsuarioId == usuarioId, cancellationToken);

        if (existing is not null)
        {
            if (existing.EsDislike)
            {
                // Ya tiene dislike
                return false;
            }

            // Cambiar like a dislike
            existing.EsDislike = true;
            existing.FechaCreacion = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        var dislike = new LikeResena
        {
            Id = Guid.NewGuid(),
            ResenaId = resenaId,
            UsuarioId = usuarioId,
            EsDislike = true,
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.LikesResena.Add(dislike);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveLikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var like = await dbContext.LikesResena
            .FirstOrDefaultAsync(l => l.ResenaId == resenaId && l.UsuarioId == usuarioId, cancellationToken);
        if (like is null)
        {
            return false;
        }

        dbContext.LikesResena.Remove(like);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<LikeCountResponse> GetLikeCountsAsync(Guid resenaId, CancellationToken cancellationToken = default)
    {
        var likes = await dbContext.LikesResena
            .AsNoTracking()
            .Where(l => l.ResenaId == resenaId)
            .GroupBy(_ => 1)
            .Select(g => new LikeCountResponse
            {
                LikeCount = g.Count(l => !l.EsDislike),
                DislikeCount = g.Count(l => l.EsDislike)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return likes ?? new LikeCountResponse();
    }

    public async Task<bool> AddUgcLikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.LikesUgc
            .FirstOrDefaultAsync(l => l.UgcId == ugcId && l.UsuarioId == usuarioId, cancellationToken);

        if (existing is not null)
        {
            if (!existing.EsDislike)
            {
                return false;
            }

            existing.EsDislike = false;
            existing.FechaCreacion = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        var like = new LikeUgc
        {
            Id = Guid.NewGuid(),
            UgcId = ugcId,
            UsuarioId = usuarioId,
            EsDislike = false,
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.LikesUgc.Add(like);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AddUgcDislikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.LikesUgc
            .FirstOrDefaultAsync(l => l.UgcId == ugcId && l.UsuarioId == usuarioId, cancellationToken);

        if (existing is not null)
        {
            if (existing.EsDislike)
            {
                return false;
            }

            existing.EsDislike = true;
            existing.FechaCreacion = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        var dislike = new LikeUgc
        {
            Id = Guid.NewGuid(),
            UgcId = ugcId,
            UsuarioId = usuarioId,
            EsDislike = true,
            FechaCreacion = DateTime.UtcNow
        };

        dbContext.LikesUgc.Add(dislike);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveUgcLikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var like = await dbContext.LikesUgc
            .FirstOrDefaultAsync(l => l.UgcId == ugcId && l.UsuarioId == usuarioId, cancellationToken);
        if (like is null)
        {
            return false;
        }

        dbContext.LikesUgc.Remove(like);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<LikeCountResponse> GetUgcLikeCountsAsync(Guid ugcId, CancellationToken cancellationToken = default)
    {
        var likes = await dbContext.LikesUgc
            .AsNoTracking()
            .Where(l => l.UgcId == ugcId)
            .GroupBy(_ => 1)
            .Select(g => new LikeCountResponse
            {
                LikeCount = g.Count(l => !l.EsDislike),
                DislikeCount = g.Count(l => l.EsDislike)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return likes ?? new LikeCountResponse();
    }
}
