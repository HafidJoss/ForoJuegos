using NuevoForo.Application.DTOs.Reviews;

namespace NuevoForo.Application.Services;

public interface IReviewService
{
    Task<ReviewResponse> CreateAsync(Guid usuarioId, ReviewCreateRequest request, CancellationToken cancellationToken = default);
    Task<ReviewResponse?> UpdateAsync(Guid id, Guid usuarioId, bool esModerador, ReviewUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewResponse>> GetByGameAsync(Guid juegoId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewResponse>> GetByUserAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
