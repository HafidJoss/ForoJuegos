using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Games;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("games")]
public class GamesController(IGameService gameService, IMemoryCache cache, IConfiguration configuration) : ControllerBase
{
    private readonly int _gamesListTtlSeconds = configuration.GetValue<int>("Cache:GamesListTtlSeconds");
    private readonly int _gameDetailTtlSeconds = configuration.GetValue<int>("Cache:GameDetailTtlSeconds");
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<JuegoResponse>> Create(JuegoCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await gameService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<JuegoResponse>> Update(Guid id, JuegoUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await gameService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await gameService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<JuegoResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"game:detail:{id}";
        if (!cache.TryGetValue(cacheKey, out JuegoResponse? juego))
        {
            juego = await gameService.GetByIdAsync(id, cancellationToken);
            if (juego is null)
            {
                return NotFound();
            }
            cache.Set(cacheKey, juego, TimeSpan.FromSeconds(_gameDetailTtlSeconds));
        }

        return Ok(juego);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<JuegoListResponse>> List([FromQuery] string? texto, [FromQuery] string? genero, [FromQuery] string? tags, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"games:list:{texto}:{genero}:{tags}:{page}:{pageSize}";
        if (!cache.TryGetValue(cacheKey, out JuegoListResponse? result))
        {
            result = await gameService.ListAsync(texto, genero, tags, page, pageSize, cancellationToken);
            cache.Set(cacheKey, result, TimeSpan.FromSeconds(_gamesListTtlSeconds));
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene lista simplificada de juegos para selector en UI (dropdown, autocomplete, etc).
    /// Retorna solo Id, Nombre, e ImagenPortadaUrl de cada juego.
    /// Ideal para componentes que necesitan mostrar juegos de forma compacta.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de juegos simplificados para selector.</returns>
    [HttpGet("select")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GameSelectDto>>> GetForSelect(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "games:select:all";
        if (!cache.TryGetValue(cacheKey, out IEnumerable<GameSelectDto>? juegos))
        {
            juegos = await gameService.GetForSelectAsync(cancellationToken);
            // Cachear por 1 hora (3600 segundos)
            cache.Set(cacheKey, juegos, TimeSpan.FromSeconds(3600));
        }

        return Ok(juegos);
    }

    [HttpGet("genres")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetGenres(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "games:genres:all";
        if (!cache.TryGetValue(cacheKey, out IEnumerable<string>? genres))
        {
            genres = await gameService.GetGenresAsync(cancellationToken);
            cache.Set(cacheKey, genres, TimeSpan.FromHours(1));
        }
        return Ok(genres);
    }

    [HttpGet("tags")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetTags(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "games:tags:all";
        if (!cache.TryGetValue(cacheKey, out IEnumerable<string>? tags))
        {
            tags = await gameService.GetTagsAsync(cancellationToken);
            cache.Set(cacheKey, tags, TimeSpan.FromHours(1));
        }
        return Ok(tags);
    }
}
