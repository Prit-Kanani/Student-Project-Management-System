using RabbitMQ.Client;

namespace ProjectGroupService.RabbitMQ;

public interface IMessageBus
{
    // A generic method to publish any type of event
    void Publish<T>(T message, string exchangeName, string eventName, IChannel? _channel);
}
