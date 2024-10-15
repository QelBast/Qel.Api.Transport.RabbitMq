namespace Qel.Api.Transport.RabbitMq.Models;


public class ClientOptions
{
    const string defaultUsername = "guest";
    const string defaultPassword = "guest";
    const string defaultHostname = "localhost";
    const int defaultPort = 5672;

    public string? Hostname { get; init; } = defaultHostname;
    public int Port { get; init; } = defaultPort;
    public string? Name { get; init; } = Guid.NewGuid().ToString();
    public required string Username { get; init; } = defaultUsername;
    public required string Password { get; init; } = defaultPassword;
}
