using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator;
using NewsAggregator.Options;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;


services.RegisterOptions(configuration);
services.RegisterHttpClients();

services.RegisterServices();
services.RegisterDbContext(configuration);

services.RegisterMessageBroker();

services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


