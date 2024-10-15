namespace Qel.Api.Transport.RabbitMq.EventBus.Models;

/// <summary>
/// Options for RabbitMq Event bus
/// </summary>
public class EventBusOptions
{
    public required string SubscriptionClientName { get; set; }
    public int RetryCount { get; set; } = 10;
}
