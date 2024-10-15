using Microsoft.Extensions.Options;
using Qel.Api.Transport.RabbitMq.Models;

namespace Qel.Api.Transport.RabbitMq;

public class Router<T>(
    IOptions<ReceiverOptions> receiverOptions,
    IMessageProcesser<T> processer,
    IOptions<SenderOptions>? senderOptions = null) : ITransportRouter
{
    readonly ReceiverOptions _receiverOptions = receiverOptions.Value;
    readonly SenderOptions? _senderOptions = senderOptions?.Value;
    readonly IMessageProcesser<T> _processer = processer;
    
    public Task AllRouteWithOneConnection(CancellationToken token)
    {
        using var connection = Modeller.CreateConnection(_receiverOptions.ClientOptions);
        using var channel = Modeller.CreateModel(connection);

        Sender sender = new (
            _senderOptions ?? throw new ArgumentNullException("Не получены настройки отправителя, хотя метод предусматривает отправку"), 
            channel);
        Receiver<T> receiver = new (
            _receiverOptions, 
            channel, 
            sender);

        receiver.Receive(_processer);
        connection.Abort();
        return Task.CompletedTask;
    }

    public Task AllRouteWithDifferentConnection(CancellationToken token)
    {
        using var receiverConnection = Modeller.CreateConnection(_receiverOptions.ClientOptions);
        using var senderConnection = Modeller.CreateConnection(
            _senderOptions?.ClientOptions ?? throw new ArgumentNullException("Не получены настройки отправителя, хотя метод предусматривает отправку"));
        using var receiverChannel = Modeller.CreateModel(receiverConnection);
        using var senderChannel = Modeller.CreateModel(senderConnection);
        
        Sender sender = new (
            _senderOptions, 
            senderChannel);
        
        Receiver<T> receiver = new (
            _receiverOptions, 
            receiverChannel, 
            sender);

        receiver.Receive(_processer);
        receiverConnection.Abort();
        senderConnection.Abort();
        return Task.CompletedTask;
    }

    public Task OnlyReceiveRoute(CancellationToken token)
    {
        using var connection = Modeller.CreateConnection(_receiverOptions.ClientOptions);
        using var channel = Modeller.CreateModel(connection);

        Receiver<T> receiver = new (
            _receiverOptions, 
            channel);

        receiver.Receive(_processer);
        connection.Abort();
        return Task.CompletedTask;
    }
}
