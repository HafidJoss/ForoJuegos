using System.ComponentModel.DataAnnotations;

namespace NuevoForo.Application.DTOs.Notifications;

public sealed class NotificationMarkReadRequest
{
    [Required]
    public Guid NotificationId { get; set; }
}
