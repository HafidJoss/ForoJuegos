using NuevoForo.Application.DTOs.Ugc;

namespace NuevoForo.Application.Services;

public interface IUgcService
{
    Task<UgcResponse> CreateAsync(Guid usuarioId, UgcCreateRequest request, CancellationToken cancellationToken = default);
    Task<UgcResponse?> UpdateAsync(Guid id, Guid usuarioId, bool esModerador, UgcUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default);
    Task<UgcResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UgcResponse>> GetByGameAsync(Guid juegoId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UgcResponse>> GetByUserAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UgcResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
