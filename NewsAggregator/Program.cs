using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator;
using NewsAggregator.Options;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpClient("rss", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "NewsAggregator/1.0");
});

services.AddHttpClient("scraper", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
    client.Timeout = TimeSpan.FromSeconds(30); // Статьи могут грузиться дольше
});

services.RegisterOptions(configuration);

services.RegisterServices();

services.RegisterMessageBroker();

services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


