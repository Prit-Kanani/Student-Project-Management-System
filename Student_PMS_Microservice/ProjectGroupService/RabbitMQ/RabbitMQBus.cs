using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProjectGroupService.RabbitMQ;

public class RabbitMQBus : IMessageBus, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel; 
    private readonly ConnectionFactory _factory;

    public RabbitMQBus(string host, string port)
    {
        _factory = new ConnectionFactory()
        {
            HostName = host,
            Port = int.Parse(port)
        };
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }
    }

    public IChannel? Get_channel<T>()
    {
        return _channel;
    }

    public async void Publish<T>(T message, string exchangeName, string eventName, IChannel? _channel)
    {
        await EnsureConnectionAsync();

        await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        // In v7, properties are handled slightly differently
        await _channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: string.Empty,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
