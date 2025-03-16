using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly string _exchange;

    public RabbitMQConsumer(IConfiguration config)
    {
        var factory = new ConnectionFactory()
        {
            HostName = config["RabbitMQ:HostName"],
            UserName = config["RabbitMQ:Username"],
            Password = config["RabbitMQ:Password"]
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _exchange = config["RabbitMQ:Exchange"];
        _queueName = config["RabbitMQ:Queue"];

        // Declare the exchange first to make sure it exists
        _channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        // Now declare the queue
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);

        // Bind the queue to the exchange
        _channel.QueueBind(_queueName, _exchange, config["RabbitMQ:RoutingKey"]);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventData = JsonSerializer.Deserialize<dynamic>(message);

            Console.WriteLine($"Received Event: {eventData}");
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }
}
