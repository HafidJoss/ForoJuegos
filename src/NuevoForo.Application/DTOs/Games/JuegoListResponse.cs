namespace NuevoForo.Application.DTOs.Games;

public sealed class JuegoListResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public IReadOnlyList<JuegoResponse> Items { get; set; } = Array.Empty<JuegoResponse>();
}
