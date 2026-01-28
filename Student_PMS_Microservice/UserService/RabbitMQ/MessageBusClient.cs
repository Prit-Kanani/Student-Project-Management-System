/*using System.Text;
using System.Text.Json;
using LMS.RequestService.DTOs;
using RabbitMQ.Client; // Ensure version 7.0+

namespace LMS.RequestService.Messaging
{
    public interface IMessageBusClient
    {
        Task PublishLeaveApproved(LeaveApprovedEvent eventDto);
    }

    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel; // v7 Rename: IModel is now IChannel

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // We create the connection only when needed to avoid blocking the constructor
        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _configuration["RabbitMQHost"],
                    Port = int.Parse(_configuration["RabbitMQPort"])
                };

                // v7: All connections are now Task-based (Async)
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Declare a Fanout exchange (broadcasts to all listeners)
                await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            }
        }

        public async Task PublishLeaveApproved(LeaveApprovedEvent eventDto)
        {
            await EnsureConnectionAsync();

            var message = JsonSerializer.Serialize(eventDto);
            var body = Encoding.UTF8.GetBytes(message);

            // v7: Sending the message is now an Async Task
            await _channel!.BasicPublishAsync(
                exchange: "trigger",
                routingKey: string.Empty, // Fanout doesn't need a routing key
                body: body);

            Console.WriteLine($"--> Sent: {message}");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}*/