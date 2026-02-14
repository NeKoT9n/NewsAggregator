namespace NewsAggregator.Options;

public class RabbitMqOptions
{
    public const string SectionName = "MessageBroker";

    public string Host { get; init; } = "localhost";
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
}