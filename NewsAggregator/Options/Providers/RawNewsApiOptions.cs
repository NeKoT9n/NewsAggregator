namespace NewsAggregator.Options.Providers;

public abstract class RawNewsApiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public IReadOnlyList<string> Categories { get; init; } = [];
}