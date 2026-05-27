using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Reviews;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("reviews")]
public class ReviewsController(IReviewService reviewService, ILikeService likeService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> Create(ReviewCreateRequest request, CancellationToken cancellationToken)
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

        var created = await reviewService.CreateAsync(usuarioId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetByGame), new { juegoId = created.JuegoId }, created);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ReviewResponse>> Update(Guid id, ReviewUpdateRequest request, CancellationToken cancellationToken)
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

        var esModerador = User.IsInRole("Admin") || User.IsInRole("Moderador");
        var updated = await reviewService.UpdateAsync(id, usuarioId.Value, esModerador, request, cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
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
        var deleted = await reviewService.DeleteAsync(id, usuarioId.Value, esModerador, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("by-game/{juegoId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> GetByGame(Guid juegoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var reviews = await reviewService.GetByGameAsync(juegoId, page, pageSize, cancellationToken);
        return Ok(reviews);
    }

    [HttpGet("by-user/{usuarioId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ReviewResponse>>> GetByUser(Guid usuarioId, CancellationToken cancellationToken)
    {
        var reviews = await reviewService.GetByUserAsync(usuarioId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> Like(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var added = await likeService.AddLikeAsync(id, usuarioId.Value, cancellationToken);
        return added ? NoContent() : Conflict(new { message = "Ya has dado like a esta reseña." });
    }

    [HttpDelete("{id:guid}/like")]
    [Authorize]
    public async Task<IActionResult> Unlike(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var removed = await likeService.RemoveLikeAsync(id, usuarioId.Value, cancellationToken);
        return removed ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/dislike")]
    [Authorize]
    public async Task<IActionResult> Dislike(Guid id, CancellationToken cancellationToken)
    {
        var usuarioId = GetUserId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        var added = await likeService.AddDislikeAsync(id, usuarioId.Value, cancellationToken);
        return added ? NoContent() : Conflict(new { message = "Ya has dado dislike a esta reseña." });
    }

    [HttpGet("{id:guid}/likes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLikeCounts(Guid id, CancellationToken cancellationToken)
    {
        var counts = await likeService.GetLikeCountsAsync(id, cancellationToken);
        return Ok(counts);
    }

    private Guid? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
