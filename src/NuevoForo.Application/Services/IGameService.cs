using NuevoForo.Application.DTOs.Games;

namespace NuevoForo.Application.Services;

public interface IGameService
{
    Task<JuegoResponse> CreateAsync(JuegoCreateRequest request, CancellationToken cancellationToken = default);
    Task<JuegoResponse?> UpdateAsync(Guid id, JuegoUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JuegoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JuegoListResponse> ListAsync(string? texto, string? genero, string? tags, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameSelectDto>> GetForSelectAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetGenresAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetTagsAsync(CancellationToken cancellationToken = default);
}
