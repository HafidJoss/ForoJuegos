namespace NuevoForo.Application.DTOs.Feed;

public class FeedItemResponse
{
    public string Type { get; set; } = string.Empty; // "review" o "ugc"
    public Guid Id { get; set; }
    public Guid ItemId { get; set; } // ResenaId o UgcId
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // Campos específicos para Review
    public int? Rating { get; set; }
    public Guid? GameId { get; set; }
    public string? GameName { get; set; }

    // Campos específicos para UGC
    public string? ImageUrl { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

    // Contadores sociales (conectados a la BD)
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int CommentCount { get; set; }
}
