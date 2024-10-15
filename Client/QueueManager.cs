using RabbitMQ.Client;

namespace Qel.Api.Transport.RabbitMq;

public class QueueManager
{
    public static bool DeclareQueue(
        IModel model, 
        string name,
        bool durable
        )
    {
        var isDeclared = model.QueueDeclare(
            queue: name,
            durable: durable,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        return isDeclared is not null;
    }
}
