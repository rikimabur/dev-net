using EshopDomain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EshopInfrastructure.DomainEvents
{
    public sealed class OrderCreatedAuditHandler
    : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedAuditHandler> _logger;

        public OrderCreatedAuditHandler(
            ILogger<OrderCreatedAuditHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(
            OrderCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Order {OrderId} created for {Email}",
                notification.OrderId,
                notification.CustomerEmail);

            return Task.CompletedTask;
        }
    }
}
