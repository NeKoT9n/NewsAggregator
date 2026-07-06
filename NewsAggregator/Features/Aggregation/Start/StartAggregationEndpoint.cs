using FastEndpoints;
using NewsAggregator.Infostructure.Services;

namespace NewsAggregator.Features.Aggregation.Start
{
    public class StartAggregationEndpoint(IServiceScopeFactory scopeFactory) : Endpoint<EmptyRequest, object>
    {
        public override void Configure()
        {
            Post("api/aggregation/start");
            Roles("Admin");
            Summary(s => {
                s.Summary = "Force news aggregation process";
                s.Description = "Triggers the scraper to immediately fetch articles from all active sources. Accessible by Admins only.";
            });
        }

        public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
        {
            _ = Task.Run(async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var aggregationService = scope.ServiceProvider.GetRequiredService<INewsAggregationService>();
                await aggregationService.AggregateNewsAsync(CancellationToken.None);
            }, CancellationToken.None);

            await Send.AcceptedAtAsync(nameof(StartAggregationEndpoint), cancellation: ct);
        }
    }
}
