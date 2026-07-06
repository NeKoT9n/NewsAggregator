namespace NewsAggregator.Infostructure.Dto;

public class AddSourceCommand
{
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Language { get; set; } = "ru";
    
    public string RssUrl { get; set; } = string.Empty; 
    
    public bool IsActive { get; set; } = true;
    
    public AddScraperConfigCommand? ScraperConfig { get; set; }
}


public class AddScraperConfigCommand
{
    
    public string? ArticleContentSelector { get; set; }
    public string? IgnoreSelector { get; set; }
    public string? ImageSelector { get; set; }
    
}