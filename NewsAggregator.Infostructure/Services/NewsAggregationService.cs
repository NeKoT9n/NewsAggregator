using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NewsAggregator.DataAccess;
using NewsAggregator.Infostructure.Options;
using NewsAggregator.Infostructure.Producers;
using NewsAggregator.Infostructure.Services.ArticleProviders;

namespace NewsAggregator.Infostructure.Services
{
    public interface INewsAggregationService
    {
        Task AggregateNewsAsync(CancellationToken cancellationToken);
    }

    public class NewsAggregationService(
        ILogger<NewsAggregationService> logger,
        NewsProducer producer,
        IScrapedArticleProvider articleProvider,
        SourceDbContext sourceContext,
        IOptions<WorkerOptions> options) : INewsAggregationService
    {
        private readonly WorkerOptions _options = options.Value;

        public async Task AggregateNewsAsync(CancellationToken cancellationToken)
        {
            var sources = await sourceContext.Sources
                .Where(s => s.IsActive)
                .Include(s => s.ScraperConfig)
                .ToListAsync(cancellationToken);

            foreach (var source in sources)
            {
                try
                {
                    logger.LogInformation("Starting article scraping for source: {SourceName}", source.Name);
                    var articles = await articleProvider.GetArticlesAsync(source);

                    foreach (var article in articles)
                    {
                        try
                        {
                            await producer.Publish(article, cancellationToken);
                            await Task.Delay(_options.MessagePublishDelay, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to publish article: {Title}", article.Title);
                        }
                    }

                    source.LastSyncAt = DateTime.UtcNow;
                    await sourceContext.SaveChangesAsync(cancellationToken);

                    logger.LogInformation("Successfully synced source: {SourceName}", source.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error while article scraping for source: {SourceName}", source.Name);
                }
            }

            logger.LogInformation("News aggregation process completed successfully.");
        }
    }
}
