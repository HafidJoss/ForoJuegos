using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Comments;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class CommentService(AppDbContext dbContext) : ICommentService
{
    public async Task<CommentResponse> CreateAsync(Guid usuarioId, CommentCreateRequest request, CancellationToken cancellationToken = default)
    {
        var comentario = new Comentario
        {
            Id = Guid.NewGuid(),
            ResenaId = request.ResenaId,
            UgcId = request.UgcId,
            UsuarioId = usuarioId,
            Texto = request.Texto.Trim(),
            FechaCreacion = DateTime.UtcNow,
            Estado = EstadoComentario.Activo
        };

        dbContext.Comentarios.Add(comentario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var user = await dbContext.Users.FindAsync(new object[] { usuarioId }, cancellationToken);
        var userNombre = user?.UserName ?? "Anónimo";

        return new CommentResponse
        {
            Id = comentario.Id,
            ResenaId = comentario.ResenaId,
            UgcId = comentario.UgcId,
            UsuarioId = comentario.UsuarioId,
            UsuarioNombre = userNombre,
            Texto = comentario.Texto,
            FechaCreacion = comentario.FechaCreacion
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default)
    {
        var comentario = await dbContext.Comentarios.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (comentario is null)
        {
            return false;
        }

        if (!esModerador && comentario.UsuarioId != usuarioId)
        {
            return false;
        }

        comentario.Estado = EstadoComentario.Eliminado;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<CommentResponse>> GetByReviewAsync(Guid resenaId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var comentarios = await dbContext.Comentarios
            .AsNoTracking()
            .Where(c => c.ResenaId == resenaId && c.Estado == EstadoComentario.Activo)
            .OrderByDescending(c => c.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                ResenaId = c.ResenaId,
                UgcId = c.UgcId,
                UsuarioId = c.UsuarioId,
                UsuarioNombre = c.Usuario.UserName ?? "Anónimo",
                Texto = c.Texto,
                FechaCreacion = c.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        return comentarios;
    }

    public async Task<IReadOnlyList<CommentResponse>> GetByUgcAsync(Guid ugcId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var comentarios = await dbContext.Comentarios
            .AsNoTracking()
            .Where(c => c.UgcId == ugcId && c.Estado == EstadoComentario.Activo)
            .OrderByDescending(c => c.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                ResenaId = c.ResenaId,
                UgcId = c.UgcId,
                UsuarioId = c.UsuarioId,
                UsuarioNombre = c.Usuario.UserName ?? "Anónimo",
                Texto = c.Texto,
                FechaCreacion = c.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        return comentarios;
    }
}
