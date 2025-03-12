using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IModel _channel;
    private readonly string _queueName;

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

        _queueName = config["RabbitMQ:Queue"];
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_queueName, config["RabbitMQ:Exchange"], config["RabbitMQ:RoutingKey"]);
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
