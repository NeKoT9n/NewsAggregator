using FastEndpoints;
using NewsAggregator.Infostructure.Services;

namespace NewsAggregator.Features.Sources.DeleteSource
{
    public class DeleteSourceEndpoint(SourceService sourceService) : Endpoint<DeleteSourceRequest>
    {
        public override void Configure()
        {
            Delete("api/source/{Id}");
            Roles("Admin");
        }

        public override async Task HandleAsync(DeleteSourceRequest req, CancellationToken ct)
        {
            var result = await sourceService.DeleteSourceAsync(req.Id, ct);

            if (result.IsFailure)
            {
                ThrowError(result.Error.Message, 400);
            }

            await Send.NoContentAsync(ct);
        }
    }
}
