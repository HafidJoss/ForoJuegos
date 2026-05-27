using NuevoForo.Application.DTOs.Notifications;

namespace NuevoForo.Application.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> ListAsync(Guid usuarioId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> MarkReadAsync(Guid usuarioId, Guid notificationId, CancellationToken cancellationToken = default);
    Task<int> MarkAllReadAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
