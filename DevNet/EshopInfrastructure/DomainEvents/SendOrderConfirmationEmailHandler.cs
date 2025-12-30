using EshopApplication.Abstractions;
using EshopDomain.Events;
using MediatR;

namespace EshopInfrastructure.DomainEvents
{
    public sealed class SendOrderConfirmationEmailHandler
     : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IEmailSender _emailSender;

        public SendOrderConfirmationEmailHandler(IEmailSender emailSender)
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
