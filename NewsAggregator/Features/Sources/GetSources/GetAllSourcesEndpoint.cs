using FastEndpoints;
using NewsAggregator.Infostructure.Services;

namespace NewsAggregator.Features.Sources.GetSources
{
    public class GetAllSourcesEndpoint(SourceService sourceService) : EndpointWithoutRequest<List<SourceResponse>>
    {
        public override void Configure()
        {
            Get("api/source/all");
            Roles("Admin"); 
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var sources = await sourceService.GetAllSourcesAsync(ct);
            var response = sources.Select(s => new SourceResponse(s.Id, s.Name, s.RssUrl, s.IsActive)).ToList();
            await Send.OkAsync(response, ct);
        }
    }
}
