namespace NewsAggregator.Options;

public class RabbitMqOptions
{
    public const string SECTION_NAME = "MessageBroker";

    public string Host { get; set; } = "localhost";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
}