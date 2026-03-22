using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsAggregator.DataAccess;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Services.ArticleProviders;
using NewsAggregator.Options;
using NewsAggregator.Producers;

namespace NewsAggregator;

public class Worker(
    ILogger<Worker> logger,
    IOptions<WorkerOptions> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly WorkerOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var producer = scope.ServiceProvider.GetRequiredService<NewsProducer>();
                var articleProvider = scope.ServiceProvider.GetRequiredService<IScrapedArticleProvider>();
                var sourceContext = scope.ServiceProvider.GetRequiredService<SourceDbContext>();

                var sources = await sourceContext.Sources
                    .Where(s => s.IsActive)
                    .Include(s => s.ScraperConfig)
                    .ToListAsync(stoppingToken);

                foreach (var source in sources)
                {
                    try
                    {
                        var articles = await articleProvider.GetArticlesAsync(source);
                        
                        foreach (var article in articles)
                        {
                            try 
                            {
                                await producer.Publish(article, stoppingToken);
                                await Task.Delay(_options.MessagePublishDelay, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to publish article: {Title}", article.Title);
                            }
                        }
                    
                        source.LastSyncAt = DateTime.UtcNow;
                        await sourceContext.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error while article scrapping");
                    }
                }
            }
            
            await Task.Delay(TimeSpan.FromMinutes(_options.SleepDelayMinutes), stoppingToken);
        }
    }
}