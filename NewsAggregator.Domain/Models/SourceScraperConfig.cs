namespace NewsAggregator.Domain.Models;

public class SourceScraperConfig
{
    public long Id { get; set; }

    public string? ArticleContentSelector { get; set; } = null!;
    public string? IgnoreSelector { get; set; } = null!;
    public string? ImageSelector { get; set; } = null!;
    
    public long SourceId { get; set; }
    public Source? Source  { get; set; }
}