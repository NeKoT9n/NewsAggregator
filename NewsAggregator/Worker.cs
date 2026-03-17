using System.Diagnostics;
using Microsoft.Extensions.Options;
using NewsAggregator.Domain.Models;
using NewsAggregator.Infostructure.Services;
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

                Source testSource = new Source()
                {
                    RssUrl = "https://www.onliner.by/feed",
                    ScraperConfig = new SourceScraperConfig
                    {
                        ArticleContentSelector = ".news-text",
                        ImageSelector = ".news-media.news-media_condensed img, .news-header__image",
                        IgnoreSelector = "script, style, .news-widget, .news-banner, .news-helpers, .news-incut, .news-reference, .news-video"
                    }
                };

                try
                {
                    var articles = await articleProvider.GetArticlesAsync(testSource);
                    
                    foreach (var article in articles)
                    {

                        await producer.Publish(article);
                    }
                    
                }
                catch (Exception)
                {
                    //TODO: log exception
                }
                
                
            }
            
            await Task.Delay(TimeSpan.FromMinutes(_options.SleepDelayMinutes), stoppingToken);
        }
    }
}