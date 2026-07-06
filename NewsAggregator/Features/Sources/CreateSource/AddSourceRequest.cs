namespace NewsAggregator.Features.Sources;

public class AddSourceRequest
{
    
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Language { get; set; } = "ru";
    
    public string RssUrl { get; set; } = string.Empty; 
    
    public bool IsActive { get; set; } = true;
    
    public AddScraperConfigRequest? ScraperConfig { get; set; }
}

public class AddScraperConfigRequest
{
    
    public string? ArticleContentSelector { get; set; }
    public string? IgnoreSelector { get; set; }
    public string? ImageSelector { get; set; }
    
}