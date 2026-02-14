namespace NewsAggregator.Options;

public class RedisOptions
{
    public const string SECTION_NAME = "Redis";

    public string ConnectionString { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "NewsAggregator_";
    public int CacheExpirationDays { get; set; } = 1;
}