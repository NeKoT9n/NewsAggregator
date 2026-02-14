using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator.Options;
using NewsAggregator.Options.Providers;
using NewsAggregator.Services;
using NewsAggregator.Services.NewsApi;

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

        public void RegisterNewsApiProviders(IConfiguration configuration)
        {
            var newsApiOptions = configuration
                .GetSection(NewsApiOptions.SectionName)
                .Get<NewsApiOptions>();

            if (newsApiOptions == null || string.IsNullOrEmpty(newsApiOptions.ApiKey)) return;
            foreach (var category in newsApiOptions.Categories)
            {
                services.AddTransient<IRawNewsProvider>(sp => 
                    new NewsApiProvider(
                        sp.GetRequiredService<IHttpClientFactory>(), 
                        newsApiOptions.ApiKey, 
                        category
                    ));
            }
        
        }
        
        public void RegisterServices()
        {
            services.AddSingleton<NewsCacheService>();
            services.AddScoped<NewsProducer>();
        }

        public void RegisterMessageBroker()
        {
            services.AddMassTransit(x =>
            {
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