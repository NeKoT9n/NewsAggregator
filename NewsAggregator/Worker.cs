using Microsoft.Extensions.Options;
using NewsAggregator.Infostructure.Options;
using NewsAggregator.Infostructure.Services;

namespace NewsAggregator;

public class Worker(
    ILogger<Worker> logger,
    IOptions<WorkerOptions> options,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly WorkerOptions _options = options.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background news aggregator worker started. Interval: every {Minutes} min.", _options.SleepDelayMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Starting scheduled automatic news aggregation...");

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var aggregationService = scope.ServiceProvider.GetRequiredService<INewsAggregationService>();

                    await aggregationService.AggregateNewsAsync(stoppingToken);
                }

                logger.LogInformation("Scheduled news aggregation completed successfully.");   
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Critical error occurred during scheduled news aggregation");
            }

            await Task.Delay(TimeSpan.FromMinutes(_options.SleepDelayMinutes), stoppingToken);
        }

        logger.LogInformation("Background news aggregator worker stopped.");
    }
}