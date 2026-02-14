using System.Diagnostics;
using Microsoft.Extensions.Options;
using NewsAggregator.Options;
using NewsAggregator.Services;

namespace NewsAggregator;

public class Worker(ILogger<Worker> logger, IOptions<WorkerOptions> options,IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly WorkerOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var producer = scope.ServiceProvider.GetRequiredService<NewsProducer>();
                var cache = scope.ServiceProvider.GetRequiredService<NewsCacheService>();

                var url = "http:/testUrl/2132434324/";
                
                if (await cache.IsAlreadyProcessed(url))
                {
                    logger.LogInformation("News has already processed: {Url}", url);
                }
                else
                {
                    await cache.MarkAsProcessed(url);
                    await producer.SendNewsToQueue("testTitle", "testContent", url);
                }
            }
            
            await Task.Delay(TimeSpan.FromMinutes(_options.SleepDelayMinutes), stoppingToken);
        }
    }
}