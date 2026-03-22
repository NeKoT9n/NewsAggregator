using System.Security.Cryptography;
using System.Text;
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
    
    public async Task<IReadOnlyList<ScrapedArticleMessage>> GetArticlesAsync(Source source)
    {
        ArgumentNullException.ThrowIfNull(source);
        
        var results = new List<ScrapedArticleMessage>();
        
        var items = await rssParser.ParseAsync(source.RssUrl);
        
        if (items.Count == 0) 
            return results;
        
        var latestItem = items.First();
        var currentFeedHash = GenerateHash($"{latestItem.Url}_{latestItem.PublishedAt}");
        
        if (source.LastProcessedHash == currentFeedHash)
            return results;

        source.LastProcessedHash = currentFeedHash;
        
        foreach (var item in items)
        {
            if (await cache.IsAlreadyProcessed(item.Url)) continue;

            try
            {
                var fullArticle = await scraper.ScrapeAsync(item, source);
                
                results.Add(new ScrapedArticleMessage
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

    private string GenerateHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}