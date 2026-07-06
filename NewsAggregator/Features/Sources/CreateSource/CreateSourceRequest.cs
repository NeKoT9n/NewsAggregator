namespace NewsAggregator.Features.Sources.CreateSource;

public class CreateSourceRequest
{
    
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string Language { get; set; } = "ru";
    
    public string RssUrl { get; set; } = string.Empty; 
    
    public bool IsActive { get; set; } = true;

    public AddScraperConfigRequest ScraperConfig { get; set; } = null!;
}

public class AddScraperConfigRequest
{

    public string ArticleContentSelector { get; set; } = null!;
    public string IgnoreSelector { get; set; } = null!;
    public string ImageSelector { get; set; } = null!;
    
}