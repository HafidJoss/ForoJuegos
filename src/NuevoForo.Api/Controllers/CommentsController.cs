using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Comments;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("comments")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentResponse>> Create(CommentCreateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var created = await commentService.CreateAsync(usuarioId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetByReview), new { resenaId = created.ResenaId }, created);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var esModerador = User.IsInRole("Admin") || User.IsInRole("Moderador");
        var deleted = await commentService.DeleteAsync(id, usuarioId.Value, esModerador, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("by-review/{resenaId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CommentResponse>>> GetByReview(Guid resenaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var comments = await commentService.GetByReviewAsync(resenaId, page, pageSize, cancellationToken);
        return Ok(comments);
    }

    [HttpGet("by-ugc/{ugcId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CommentResponse>>> GetByUgc(Guid ugcId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var comments = await commentService.GetByUgcAsync(ugcId, page, pageSize, cancellationToken);
        return Ok(comments);
    }

    private Guid? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
