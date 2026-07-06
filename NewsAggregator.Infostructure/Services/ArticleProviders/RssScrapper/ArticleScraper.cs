using AngleSharp;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Dto;
using Shared.Common.Validation;

namespace NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;

public class ArticleScraper(IHttpClientFactory clientFactory)
{
    public async Task<Result<ScrappedArticle, Error>> ScrapeAsync(RssItem rssItem, SourceScraperConfig config)
    {
        var client = clientFactory.CreateClient("scraper");

        var response = await client.GetAsync(rssItem.Url);
        
        if (!response.IsSuccessStatusCode)
            return Errors.General.Failure($"Fail to load page");

        var html = await response.Content.ReadAsStringAsync();
        
        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(html));
        
        var contentNode = document.QuerySelector(config.ArticleContentSelector);
        
        if (contentNode == null)
            return Errors.General.Validation($"Content selector is invalid'", nameof(config.ArticleContentSelector));
        
        if (!string.IsNullOrEmpty(config.IgnoreSelector))
        {
            var junk = contentNode.QuerySelectorAll(config.IgnoreSelector);
            foreach (var el in junk) el.Remove();
        }

        var paragraphs = contentNode.QuerySelectorAll("p")
            .Select(p => p.TextContent.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();
        
        if (paragraphs.Count == 0)
            return Errors.General.Validation("There are not (<p>) in this block");
        
        var cleanText = string.Join("\n\n", paragraphs);

        var imageUrl = document.QuerySelector("meta[property='og:image']")?.GetAttribute("content")
                       ?? document.QuerySelector(config.ImageSelector ?? "img")?.GetAttribute("src");

        return new ScrappedArticle(
            rssItem.Title,
            cleanText,
            imageUrl,
            rssItem.Url,
            rssItem.PublishedAt
        );
        
    }
}