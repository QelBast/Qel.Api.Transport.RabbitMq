using System.Text;
using Qel.Api.Transport.Generic;
using Qel.Api.Transport.RabbitMq.Models;
using RabbitMQ.Client;

namespace Qel.Api.Transport.RabbitMq;

public class Sender(SenderOptions options, IModel model) : ITransportSender
{
    readonly SenderOptions _options = options;

    readonly IModel _model = model;

    public void Send<T>(BaseMessage<T> message)
    {
        Send(_model, message);
    }

    public void Send<T>(IModel channel, T message)
    {
        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: _options.IsMandatory,
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(message!.ToString() ?? string.Empty)
        );
    }
}
