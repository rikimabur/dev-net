using EshopDomain.Common;
using EshopDomain.Events;
using EshopDomain.ValueObjects;

namespace EshopDomain.Aggregates
{
    public class Order : AggregateRoot
    {
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public Email CustomerEmail { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Order(Email email)
        {
            Id = Guid.NewGuid();
            CustomerEmail = email;
            CreatedAt = DateTime.UtcNow;
        }

        public static Order Create(string email)
        {
            var order = new Order(Email.Create(email));
            order.AddEvent(new OrderCreatedEvent(order.Id, email));
            return order;
        }

        public void AddItem(string productName, decimal price, int quantity)
        {
            if (quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");
            if (price <= 0)
                throw new InvalidOperationException("Price must be greater than zero");
            if (string.IsNullOrWhiteSpace(productName))
                throw new InvalidOperationException("Product name is required");

            _items.Add(new OrderItem(productName, price, quantity));
            AddEvent(new OrderItemAddedEvent(Id, productName, quantity, price));
        }

        public decimal Total() => _items.Sum(x => x.Price * x.Quantity);
    }
}
