namespace EshopDomain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public OrderCreatedEvent(Guid orderId) => OrderId = orderId;
    }
}
