using MassTransit;
using Shared.Models.Messages;

namespace NewsAggregator.Services;

public class NewsProducer(IPublishEndpoint publishEndpoint, ILogger<NewsProducer> logger)
{
    public async Task SendNewsToQueue(string title, string content, string url)
    {
        logger.LogInformation("News is publishing: {Title}", title);

        await publishEndpoint.Publish(new RawNewsScraped
        {
            Title = title,
            Content = content,
            Url = url,
            ScrapedAt = DateTime.UtcNow
        });
    }
}