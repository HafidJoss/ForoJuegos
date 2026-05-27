using NuevoForo.Application.DTOs.Likes;

namespace NuevoForo.Application.Services;

public interface ILikeService
{
    Task<bool> AddLikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<bool> AddDislikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<bool> RemoveLikeAsync(Guid resenaId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<LikeCountResponse> GetLikeCountsAsync(Guid resenaId, CancellationToken cancellationToken = default);

    Task<bool> AddUgcLikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<bool> AddUgcDislikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<bool> RemoveUgcLikeAsync(Guid ugcId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<LikeCountResponse> GetUgcLikeCountsAsync(Guid ugcId, CancellationToken cancellationToken = default);
}
