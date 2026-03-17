using System.Net.Http.Json;
using Shared.Models.Messages;

namespace NewsAggregator.Services.NewsApi;

public class NewsApiProvider : IRawNewsProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _category;

    private readonly string _url;

    public NewsApiProvider(IHttpClientFactory httpClientFactory, string apiKey, string category)
    {
        _httpClientFactory = httpClientFactory;
        _category = category;
        
        _url = $"https://newsapi.org/v2/top-headlines?category={_category.ToLower()}&apiKey={apiKey}";
    }


    public async Task<IReadOnlyList<RawNewsScraped>> GetNewsAsync()
    {

        var client = _httpClientFactory.CreateClient("NewsApi");
        
        try
        {
            var response = await client.GetFromJsonAsync<NewsApiResponse>(_url);
            
            if (response?.Articles == null)
                return [];

            return response.Articles.Select(a => new RawNewsScraped
            {
                Title = a.Title,
                Content = a.Description,
                Url = a.Url,
                CategoryName = _category,
                ScrapedAt = a.PublishedAt
            }).ToList();
        }
        catch (Exception)
        {
            // ignored
        }

        return [];
    }
}

public class NewsApiResponse
{
    public required string Status { get; init; }
    public required List<ArticleDto> Articles { get; init; }
}

public class ArticleDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Url { get; init; }
    public required DateTime PublishedAt { get; init; }
    public required SourceDto Source { get; init; }
}

public class SourceDto
{
    public required string Name { get; init; }
}