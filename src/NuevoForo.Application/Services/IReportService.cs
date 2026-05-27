using NuevoForo.Application.DTOs.Reports;
using NuevoForo.Domain.Enums;

namespace NuevoForo.Application.Services;

public interface IReportService
{
    Task<ReportResponse> CreateAsync(Guid usuarioId, ReportCreateRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportResponse>> ListAsync(EstadoReporte? estado, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ReportResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ReportResponse?> ModerateAsync(Guid id, Guid moderadorId, AccionModeracion accion, bool rechazar, CancellationToken cancellationToken = default);
}
