using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Services.Cache;
using Shared.Models.Messages;

namespace NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;

public class ArticleProvider(
    RssParser rssParser, 
    ArticleScraper scraper,
    NewsCacheService cache)
    : IScrapedArticleProvider
{
    
    public async Task<IReadOnlyList<ScrapedArticleDto>> GetArticlesAsync(Source source)
    {
        ArgumentNullException.ThrowIfNull(source);
        
        var results = new List<ScrapedArticleDto>();
        
        var items = await rssParser.ParseAsync(source.RssUrl);

        foreach (var item in items)
        {
            if (await cache.IsAlreadyProcessed(item.Url)) continue;

            try
            {
                var fullArticle = await scraper.ScrapeAsync(item, source);
                
                results.Add(new ScrapedArticleDto
                {
                    SourceId = source.Id,
                    Title = fullArticle.Title,
                    Content = fullArticle.FullText,
                    OriginalUrl = fullArticle.OriginalUrl,
                    ImageUrl = fullArticle.MainImageUrl ?? string.Empty,
                    PublishedAt = fullArticle.PublishDate,
                    ScrapedAt = DateTime.UtcNow,
                    CategoryName = item.Category,
                    Language = source.Language 
                });
                
                await cache.MarkAsProcessed(item.Url);

                await Task.Delay(500);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return results;
    }
}