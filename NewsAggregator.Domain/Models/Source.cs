namespace NewsAggregator.Domain.Models;

public class Source
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Language { get; set; } = "ru";
    
    public string RssUrl { get; set; } = string.Empty; 
    
    public DateTime? LastSyncAt { get; set; } 
    
    public bool IsActive { get; set; } = true;
    public string? LastProcessedHash { get; set; }
    
    public SourceScraperConfig? ScraperConfig { get; set; }
}

public class SourceScraperConfig
{
    public long Id { get; set; }
    
    public string? ArticleContentSelector { get; set; }
    public string? IgnoreSelector { get; set; }
    public string? ImageSelector { get; set; }
    
    public long SourceId { get; set; }
    public Source? Source  { get; set; }
}