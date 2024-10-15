using Qel.Api.Transport.Generic;
using Qel.Api.Transport.RabbitMq.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Qel.Api.Transport.RabbitMq;

public class Receiver<T>(ReceiverOptions options, IModel model, ITransportSender? sender = null) : ITransportReceiver<T>
{
    readonly ReceiverOptions _options = options;
    readonly IModel _model = model;

    public void Receive(IMessageProcesser<T> processer)
    {
        Receive(_model, processer);
    }

    void Receive(IModel channel, IMessageProcesser<T> processer)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = TransportMessageConverter.GetUTF8String(body);
            
            BaseMessage<T> messageToProcess = new(message);
            processer.Process(ref messageToProcess);
            sender?.Send(messageToProcess);
        };
        channel.BasicConsume(queue: _options.QueueName,
                            autoAck: _options.IsAutoAck,
                            consumerTag: _options.ConsumerTag,
                            consumer: consumer);   
    }
}
