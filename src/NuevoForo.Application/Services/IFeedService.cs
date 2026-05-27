using NuevoForo.Application.DTOs.Feed;

namespace NuevoForo.Application.Services;

public interface IFeedService
{
    Task<IReadOnlyList<FeedItemResponse>> GetFeedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
