using EshopApplication.Common.Interfaces;
using EshopApplication.Orders.IntegrationEvents;
using EshopDomain.Events;
using MediatR;

namespace EshopApplication.Orders.Events
{
    public sealed class PublishOrderCreatedIntegrationEventHandler
    : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IEventBus _eventBus;

        public PublishOrderCreatedIntegrationEventHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task Handle(
            OrderCreatedEvent notification,
            CancellationToken cancellationToken)
        {
           await _eventBus.PublishAsync(new OrderCreatedIntegrationEvent(
                notification.OrderId));

        }
    }
}
