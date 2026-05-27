using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Games;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Entities;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class GameService(AppDbContext dbContext) : IGameService
{
    public async Task<JuegoResponse> CreateAsync(JuegoCreateRequest request, CancellationToken cancellationToken = default)
    {
        var juego = new Juego
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            GeneroPrincipal = request.GeneroPrincipal?.Trim(),
            Tags = request.Tags?.Trim(),
            FechaLanzamiento = request.FechaLanzamiento,
            Plataforma = request.Plataforma?.Trim(),
            ImagenPortadaUrl = request.ImagenPortadaUrl?.Trim()
        };

        dbContext.Juegos.Add(juego);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(juego);
    }

    public async Task<JuegoResponse?> UpdateAsync(Guid id, JuegoUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var juego = await dbContext.Juegos.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (juego is null)
        {
            return null;
        }

        juego.Nombre = request.Nombre.Trim();
        juego.Descripcion = request.Descripcion?.Trim();
        juego.GeneroPrincipal = request.GeneroPrincipal?.Trim();
        juego.Tags = request.Tags?.Trim();
        juego.FechaLanzamiento = request.FechaLanzamiento;
        juego.Plataforma = request.Plataforma?.Trim();
        juego.ImagenPortadaUrl = request.ImagenPortadaUrl?.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToResponse(juego);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var juego = await dbContext.Juegos.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        if (juego is null)
        {
            return false;
        }

        dbContext.Juegos.Remove(juego);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<JuegoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var juego = await dbContext.Juegos.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
        return juego is null ? null : MapToResponse(juego);
    }

    public async Task<JuegoListResponse> ListAsync(string? texto, string? genero, string? tags, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var query = dbContext.Juegos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var criterio = texto.Trim().ToLower();
            query = query.Where(j => j.Nombre.ToLower().Contains(criterio) || (j.Descripcion != null && j.Descripcion.ToLower().Contains(criterio)));
        }

        if (!string.IsNullOrWhiteSpace(genero))
        {
            var criterio = genero.Trim().ToLower();
            query = query.Where(j => j.GeneroPrincipal != null && j.GeneroPrincipal.ToLower().Contains(criterio));
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var criterio = tags.Trim().ToLower();
            query = query.Where(j => j.Tags != null && j.Tags.ToLower().Contains(criterio));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(j => j.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JuegoResponse
            {
                Id = j.Id,
                Nombre = j.Nombre,
                Descripcion = j.Descripcion,
                GeneroPrincipal = j.GeneroPrincipal,
                Tags = j.Tags,
                FechaLanzamiento = j.FechaLanzamiento,
                Plataforma = j.Plataforma,
                ImagenPortadaUrl = j.ImagenPortadaUrl
            })
            .ToListAsync(cancellationToken);

        return new JuegoListResponse
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = items
        };
    }

    public async Task<IEnumerable<string>> GetGenresAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Juegos
            .AsNoTracking()
            .Where(j => j.GeneroPrincipal != null && j.GeneroPrincipal != "")
            .Select(j => j.GeneroPrincipal!)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var rawTags = await dbContext.Juegos
            .AsNoTracking()
            .Where(j => j.Tags != null && j.Tags != "")
            .Select(j => j.Tags!)
            .ToListAsync(cancellationToken);

        return rawTags
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .Take(50)
            .ToList();
    }

    /// <summary>
    /// Obtiene lista de todos los juegos con información simplificada para selector en UI.
    /// Solo retorna Id, Nombre, e ImagenPortadaUrl ordenados alfabéticamente.
    /// </summary>
    public async Task<IEnumerable<GameSelectDto>> GetForSelectAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Juegos
            .AsNoTracking()
            .OrderBy(j => j.Nombre)
            .Select(j => new GameSelectDto
            {
                Id = j.Id,
                Nombre = j.Nombre,
                ImagenPortadaUrl = j.ImagenPortadaUrl
            })
            .ToListAsync(cancellationToken);
    }

    private static JuegoResponse MapToResponse(Juego juego)
    {
        return new JuegoResponse
        {
            Id = juego.Id,
            Nombre = juego.Nombre,
            Descripcion = juego.Descripcion,
            GeneroPrincipal = juego.GeneroPrincipal,
            Tags = juego.Tags,
            FechaLanzamiento = juego.FechaLanzamiento,
            Plataforma = juego.Plataforma,
            ImagenPortadaUrl = juego.ImagenPortadaUrl
        };
    }
}
