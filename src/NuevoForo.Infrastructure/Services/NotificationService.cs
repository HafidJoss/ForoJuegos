using Microsoft.EntityFrameworkCore;
using NuevoForo.Application.DTOs.Notifications;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Enums;
using NuevoForo.Infrastructure.Data;

namespace NuevoForo.Infrastructure.Services;

public class NotificationService(AppDbContext dbContext) : INotificationService
{
    public async Task<IReadOnlyList<NotificationResponse>> ListAsync(Guid usuarioId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var notifications = await dbContext.Notificaciones
            .AsNoTracking()
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationResponse
            {
                Id = n.Id,
                Tipo = n.Tipo,
                Mensaje = n.Mensaje,
                Leida = n.Leida,
                FechaCreacion = n.FechaCreacion
            })
            .ToListAsync(cancellationToken);

        return notifications;
    }

    public async Task<bool> MarkReadAsync(Guid usuarioId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificationId && n.UsuarioId == usuarioId, cancellationToken);
        if (notification is null)
        {
            return false;
        }

        if (!notification.Leida)
        {
            notification.Leida = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<int> MarkAllReadAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var notifications = await dbContext.Notificaciones
            .Where(n => n.UsuarioId == usuarioId && !n.Leida)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Leida = true;
        }

        if (notifications.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return notifications.Count;
    }
}
