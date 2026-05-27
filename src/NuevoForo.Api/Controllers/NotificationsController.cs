using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Notifications;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var notifications = await notificationService.ListAsync(usuarioId.Value, page, pageSize, cancellationToken);
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var updated = await notificationService.MarkReadAsync(usuarioId.Value, id, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpPost("read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var count = await notificationService.MarkAllReadAsync(usuarioId.Value, cancellationToken);
        return Ok(new { updated = count });
    }

    private Guid? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
