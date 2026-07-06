using FastEndpoints;
using NewsAggregator.Infostructure.Services;

namespace NewsAggregator.Features.Sources.ToggleActive
{
    public class ToggleActiveEndpoint(SourceService sourceService) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Patch("api/source/{id}/toggle");
            Roles("Admin");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<long>("id");
            var result = await sourceService.ToggleSourceStatusAsync(id, ct);

            if (result.IsFailure) ThrowError(result.Error.Message);
            await Send.NoContentAsync(ct);
        }
    }
}
