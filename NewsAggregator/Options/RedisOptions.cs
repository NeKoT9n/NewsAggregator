namespace NewsAggregator.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";

    public string ConnectionString { get; init; } = "localhost:6379";
    public string InstanceName { get; init; } = "NewsAggregator_";
    public int CacheExpirationDays { get; init; } = 1;
}