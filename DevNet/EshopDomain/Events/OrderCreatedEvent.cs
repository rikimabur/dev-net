using EshopDomain.Common;

namespace EshopDomain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string CustomerEmail { get; }
        public OrderCreatedEvent(Guid orderId, string customerEmail)
        {
            OrderId = orderId;
            CustomerEmail = customerEmail;
        }
    }
}
