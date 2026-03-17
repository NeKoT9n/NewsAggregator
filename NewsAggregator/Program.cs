using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator;
using NewsAggregator.Options;
using NewsAggregator.Services;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpClient("NewsApi", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "MyNewsApp/1.0");
});

services.RegisterOptions(configuration);

services.RegisterServices();
services.RegisterNewsApiProviders(configuration);

services.RegisterMessageBroker();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


