using MassTransit;
using Shared.Models.Messages;

namespace NewsAggregator.Producers;

public class NewsProducer(IPublishEndpoint publishEndpoint, ILogger<NewsProducer> logger)
{
    public async Task Publish(ScrapedArticleDto article)
    {
        logger.LogInformation("News is publishing: {article.Title}", article.Title);

        await publishEndpoint.Publish(article);
    }
}