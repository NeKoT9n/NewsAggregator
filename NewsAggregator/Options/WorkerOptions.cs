namespace NewsAggregator.Options;

public class WorkerOptions
{
    public const string SectionName = "Worker";

    public int SleepDelayMinutes { get; init; } = 30;
    public int MessagePublishDelay { get; init; } = 200;
}