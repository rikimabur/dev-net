using EshopApplication.Abstractions;
using EshopDomain.Events;
using MediatR;

namespace EshopApplication.Orders.Events
{
    public sealed class OrderCreatedEventHandler
    : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IEmailSender _emailSender;

        public OrderCreatedEventHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Handle(
            OrderCreatedEvent notification,
            CancellationToken cancellationToken)
        {
            await _emailSender.SendAsync(
                notification.CustomerEmail,
                "Order Confirmation",
                $"Your order {notification.OrderId} has been created.",
                cancellationToken);
        }
    }
}
