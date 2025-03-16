using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Messaging;

public class RabbitMQPublisher : IRabbitMQPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchange;

    public RabbitMQPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory()
        {
            HostName = config["RabbitMQ:HostName"],
            UserName = config["RabbitMQ:Username"],
            Password = config["RabbitMQ:Password"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _exchange = config["RabbitMQ:Exchange"];

        // Ensure durability is set properly
        _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public void PublishMessage<T>(T message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(
            exchange: _exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );
    }
}
