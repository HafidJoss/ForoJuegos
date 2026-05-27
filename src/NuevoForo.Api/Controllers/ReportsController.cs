using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuevoForo.Application.DTOs.Reports;
using NuevoForo.Application.Services;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Api.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReportResponse>> Create(ReportCreateRequest request, CancellationToken cancellationToken)
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

        var created = await reportService.CreateAsync(usuarioId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderador")]
    public async Task<ActionResult<IReadOnlyList<ReportResponse>>> List([FromQuery] EstadoReporte? estado, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var reportes = await reportService.ListAsync(estado, page, pageSize, cancellationToken);
        return Ok(reportes);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Moderador")]
    public async Task<ActionResult<ReportResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reporte = await reportService.GetByIdAsync(id, cancellationToken);
        return reporte is null ? NotFound() : Ok(reporte);
    }

    [HttpPost("{id:guid}/action")]
    [Authorize(Roles = "Admin,Moderador")]
    public async Task<ActionResult<ReportResponse>> Moderate(Guid id, ModerationActionRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var moderadorId = GetUserId();
        if (moderadorId is null)
        {
            return Unauthorized();
        }

        var updated = await reportService.ModerateAsync(id, moderadorId.Value, request.AccionTomada, request.Rechazar, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    private Guid? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
