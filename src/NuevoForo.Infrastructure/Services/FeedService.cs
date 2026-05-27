using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Feed;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class FeedService(AppDbContext dbContext) : IFeedService
{
    public async Task<IReadOnlyList<FeedItemResponse>> GetFeedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var takeCount = page * pageSize;

        var reseñas = await dbContext.Resenas
            .AsNoTracking()
            .Include(r => r.Usuario)
            .Include(r => r.Juego)
            .Include(r => r.Likes)
            .Include(r => r.Comentarios)
            .Where(r => r.Estado == EstadoResena.Activa)
            .OrderByDescending(r => r.FechaCreacion)
            .Take(takeCount)
            .Select(r => new FeedItemResponse
            {
                Type = "review",
                Id = r.Id,
                ItemId = r.Id,
                Title = r.Juego.Nombre,
                Content = r.Texto,
                AuthorId = r.UsuarioId,
                AuthorName = r.Usuario.UserName ?? "Anónimo",
                Date = r.FechaCreacion,
                Rating = r.Rating,
                GameId = r.JuegoId,
                GameName = r.Juego.Nombre,
                LikeCount = r.Likes.Count(l => !l.EsDislike),
                DislikeCount = r.Likes.Count(l => l.EsDislike),
                CommentCount = r.Comentarios.Count(c => c.Estado == EstadoComentario.Activo)
            })
            .ToListAsync(cancellationToken);

        var ugcList = await dbContext.ContenidosUgc
            .AsNoTracking()
            .Include(u => u.Usuario)
            .Include(u => u.Juego)
            .Include(u => u.Likes)
            .Include(u => u.Comentarios)
            .Where(u => u.Estado == EstadoContenidoUgc.Publicado)
            .OrderByDescending(u => u.FechaSubida)
            .Take(takeCount)
            .Select(u => new FeedItemResponse
            {
                Type = "ugc",
                Id = u.Id,
                ItemId = u.Id,
                Title = u.Titulo,
                Content = u.Descripcion,
                AuthorId = u.UsuarioId,
                AuthorName = u.Usuario.UserName ?? "Anónimo",
                Date = u.FechaSubida,
                GameId = u.JuegoId,
                GameName = u.Juego != null ? u.Juego.Nombre : null,
                ImageUrl = u.FotoPath,
                FileUrl = u.ArchivoUrl,
                FileName = u.ArchivoNombre,
                LikeCount = u.Likes.Count(l => !l.EsDislike),
                DislikeCount = u.Likes.Count(l => l.EsDislike),
                CommentCount = u.Comentarios.Count(c => c.Estado == EstadoComentario.Activo)
            })
            .ToListAsync(cancellationToken);

        var combined = reseñas.Concat(ugcList)
            .OrderByDescending(x => x.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return combined;
    }
}
