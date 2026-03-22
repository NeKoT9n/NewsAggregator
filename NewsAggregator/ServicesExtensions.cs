using AngleSharp;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsAggregator.DataAccess;
using NewsAggregator.Infostructure.Options;
using NewsAggregator.Infostructure.Services;
using NewsAggregator.Infostructure.Services.ArticleProviders;
using NewsAggregator.Infostructure.Services.ArticleProviders.RssScrapper;
using NewsAggregator.Infostructure.Services.Cache;
using NewsAggregator.Options;
using NewsAggregator.Producers;
using StackExchange.Redis;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace NewsAggregator;

public static class ServicesExtensions
{
    extension(IServiceCollection services)
    {
        public void RegisterOptions(IConfiguration configuration)
        {
            services.Configure<WorkerOptions>(
                configuration.GetSection(WorkerOptions.SectionName));

            services.Configure<RabbitMqOptions>(
                configuration.GetSection(RabbitMqOptions.SectionName));

            services.Configure<RedisOptions>(
                configuration.GetSection(RedisOptions.SectionName));
        
        }
        
        public void RegisterServices()
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
                return ConnectionMultiplexer.Connect(options.ConnectionString);
            });
            
            services.AddScoped<NewsCacheService>();
            services.AddScoped<NewsProducer>();
            services.AddScoped<RssParser>();
            services.AddScoped<ArticleScraper>();
            services.AddScoped<IScrapedArticleProvider, ArticleProvider>();
        }

        public void RegisterHttpClients()
        {
            services.AddHttpClient("rss", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "NewsAggregator/1.0");
            });

            services.AddHttpClient("scraper", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
                client.Timeout = TimeSpan.FromSeconds(30); 
            });
        }

        public void RegisterDbContext(ConfigurationManager configuration)
        {
            services.AddDbContext<SourceDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString(nameof(SourceDbContext))));
        }

        public void RegisterMessageBroker()
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                
                x.UsingRabbitMq((context, cfg) =>
                {
                    var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        
                    cfg.Host(options.Host, options.VirtualHost,h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}