namespace NewsAggregator.Features.Sources.GetSources
{
    public record SourceResponse(long Id, string Name, string RssUrl, bool IsActive);
}
