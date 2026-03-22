namespace Shared.Models.Messages;

public record ScrapedArticleMessage
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public long SourceId { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Description { get; init; }
    
    public string OriginalUrl { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string CategoryName { get; init; } = "General";
    public string Language { get; init; } = "ru";
    
    public DateTime PublishedAt { get; init; }
    public DateTime ScrapedAt { get; init; }
}