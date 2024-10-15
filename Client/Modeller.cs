using Qel.Api.Transport.RabbitMq.Models;
using RabbitMQ.Client;

namespace Qel.Api.Transport.RabbitMq;

public static class Modeller
{
    public static IConnection CreateConnection(ClientOptions options)
    {
        ICredentialsProvider creds = new BasicCredentialsProvider(
                name: options.Name,
                userName: options.Username,
                password: options.Password);

        ConnectionFactory factory = new () 
        {
            CredentialsProvider = creds
        };

        SslOption sslOption = new("", "", false);

        List<AmqpTcpEndpoint> endpoints = [new (
            hostName: options.Hostname,
            portOrMinusOne: options.Port,
            ssl: sslOption,
            maxMessageSize: 0u) ];

        return factory.CreateConnection(endpoints: endpoints);
    }
    
    public static IModel CreateModel(IConnection connection)
    {
        return connection.CreateModel();
    }
}
