using System.ServiceModel.Syndication;
using System.Xml;
using NewsAggregator.Infostructure.Dto;

namespace NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;

public class RssParser(IHttpClientFactory httpClientFactory)
{
    public async Task<List<RssItem>> ParseAsync(string rssUrl)
    {
        using var client = httpClientFactory.CreateClient("rss");
        await using var response = await client.GetStreamAsync(rssUrl);
        
        using var reader = XmlReader.Create(response, new XmlReaderSettings 
        { 
            Async = true
        });
        
        var feed = SyndicationFeed.Load(reader);
        
        return feed.Items.Select(item => new RssItem(
                item.Title?.Text ?? "No Title",
                item.Categories.FirstOrDefault()?.Name ?? "General",
                item.Links.FirstOrDefault(l => l.RelationshipType == "alternate")?.Uri.ToString() 
                ?? item.Links.FirstOrDefault()?.Uri.ToString() 
                ?? string.Empty,
                item.PublishDate.DateTime
            ))
            .Where(x => !string.IsNullOrEmpty(x.Url)) 
            .ToList();
    }
}