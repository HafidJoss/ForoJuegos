using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Feed;
using NuevoForo.Application.Services;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("feed")]
public class FeedController(IFeedService feedService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<FeedItemResponse>>> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var feed = await feedService.GetFeedAsync(page, pageSize, cancellationToken);
        return Ok(feed);
    }
}
