using MassTransit;
using Microsoft.Extensions.Options;
using NewsAggregator;
using NewsAggregator.Options;
using NewsAggregator.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(
    builder.Configuration.GetSection(WorkerOptions.SECTION_NAME));

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SECTION_NAME));

builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection(RedisOptions.SECTION_NAME));

builder.Services.AddSingleton<NewsCacheService>();
builder.Services.AddScoped<NewsProducer>();

builder.Services.AddMassTransit(x =>
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

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();


