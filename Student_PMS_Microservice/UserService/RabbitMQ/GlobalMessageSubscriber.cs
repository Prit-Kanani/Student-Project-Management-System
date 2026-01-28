/*using System.Text;
using System.Text.Json;

namespace ProjectGroupService.RabbitMQ;

public class GlobalMessageSubscriber<T, THandler>(
    IServiceProvider serviceProvider,
    IConfiguration config,
    string exchangeName
) : BackgroundService where THandler : IIntegrationEventHandler<T>
{
    private IConnection? _connection;
    private IChannel? _channel; 

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = config["RabbitMQHost"],
            Port = int.Parse(config["RabbitMQPort"])
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout, cancellationToken: stoppingToken);

        var queueDeclareResponse = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);
        string queueName = queueDeclareResponse.QueueName;

        await _channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: string.Empty, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<T>(message);

            if (@event != null)
            {
                var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();
                await handler.Handle(@event);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}

public interface IIntegrationEventHandler<in T>
{
    Task Handle(T @event);
}*/