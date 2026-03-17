using AngleSharp;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Dto;

namespace NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;

public class ArticleScraper(IHttpClientFactory clientFactory)
{
    public async Task<ScrappedArticle> ScrapeAsync(RssItem rssItem, Source source)
    {
        var client = clientFactory.CreateClient("scraper");

        var html = await client.GetStringAsync(rssItem.Url);

        var scraperConfig = source.ScraperConfig;

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(html));

        var contentNode = document.QuerySelector(scraperConfig.ArticleContentSelector ?? "article");

        if (contentNode == null)
            return new ScrappedArticle(rssItem.Title, "Content not found", null, rssItem.Url, rssItem.PublishDate);


        if (!string.IsNullOrEmpty(scraperConfig.IgnoreSelector))
        {
            var junk = contentNode.QuerySelectorAll(scraperConfig.IgnoreSelector);
            foreach (var el in junk) el.Remove();
        }

        var paragraphs = contentNode.QuerySelectorAll("p")
            .Select(p => p.TextContent.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t));

        var cleanText = string.Join("\n\n", paragraphs);

        var imageUrl = document.QuerySelector("meta[property='og:image']")?.GetAttribute("content")
                       ?? document.QuerySelector(scraperConfig.ImageSelector ?? "img")?.GetAttribute("src");

        return new ScrappedArticle(
            rssItem.Title,
            cleanText,
            imageUrl,
            rssItem.Url,
            rssItem.PublishDate
        );
        
    }
}