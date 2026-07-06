using FastEndpoints;
using FastEndpoints.Swagger;
using NewsAggregator;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;


services.RegisterOptions(configuration);
services.RegisterHttpClients();

services.AddAuthentication(configuration);
services.RegisterServices();
services.RegisterDbContext(configuration);

services.RegisterMessageBroker();

services.AddHostedService<Worker>();

services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "News Aggregator API";
            s.Version = "v1";

            s.EnableJWTBearerAuth();
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var connectionString = app.Configuration.GetConnectionString("AggregatorDb");
Console.WriteLine($"[DEBUG] Current ConnectionString: {connectionString}");

if (app.Environment.IsDevelopment())
{
    app.UseFastEndpoints();
    app.UseSwaggerGen();
}

app.MapGet("/", () => "Aggregator is alive!");

app.Run();


