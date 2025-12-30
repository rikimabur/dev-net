using EshopDomain.Common;

namespace EshopDomain.Events
{
    public class OrderItemAddedEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public string ProductName { get; }
        public int Quantity { get; }
        public decimal Price { get; }

        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public OrderItemAddedEvent(Guid orderId, string productName, int quantity, decimal price)
        {
            OrderId = orderId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }
}
