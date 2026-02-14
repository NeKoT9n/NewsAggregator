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
                var providers = scope.ServiceProvider.GetServices<IRawNewsProvider>();

                foreach (var provider in providers)
                {
                    try
                    {
                        var news = await provider.GetNewsAsync();
                        foreach (var item in news)
                        {
                            if (await cache.IsAlreadyProcessed(item.Url)) continue;

                            await producer.SendNewsToQueue(item.Title, item.Content, item.Url, item.CategoryName);
                            await cache.MarkAsProcessed(item.Url);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: log exception
                    }
                }
                
            }
            
            await Task.Delay(TimeSpan.FromMinutes(_options.SleepDelayMinutes), stoppingToken);
        }
    }
}