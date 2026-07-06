namespace NewsAggregator.Domain.Models;

public class SourceScraperConfig
{
    public long Id { get; set; }
    
    public string? ArticleContentSelector { get; set; }
    public string? IgnoreSelector { get; set; }
    public string? ImageSelector { get; set; }
    
    public long SourceId { get; set; }
    public Source? Source  { get; set; }
}