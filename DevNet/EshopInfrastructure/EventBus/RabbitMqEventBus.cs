using EshopApplication.Common.Interfaces;

namespace EshopInfrastructure.EventBus;
public sealed class RabbitMqEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
        where TEvent : class
    {
        // Serialize
        // Publish to RabbitMQ / Kafka / Azure Service Bus
        return Task.CompletedTask;
    }
}