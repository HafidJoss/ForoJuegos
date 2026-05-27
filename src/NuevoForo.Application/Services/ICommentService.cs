using NuevoForo.Application.DTOs.Comments;

namespace NuevoForo.Application.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateAsync(Guid usuarioId, CommentCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid usuarioId, bool esModerador, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentResponse>> GetByReviewAsync(Guid resenaId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentResponse>> GetByUgcAsync(Guid ugcId, int page, int pageSize, CancellationToken cancellationToken = default);
}
