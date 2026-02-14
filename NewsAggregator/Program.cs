using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator;
using NewsAggregator.Options;
using NewsAggregator.Services;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddHttpClient();
services.RegisterOptions(configuration);

services.RegisterServices();
services.RegisterNewsApiProviders(configuration);

services.RegisterMessageBroker();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


