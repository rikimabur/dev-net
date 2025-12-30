using EshopDomain.Aggregates;
using EshopDomain.Events;
using EshopDomain.Repositories;

namespace EshopApplication.Orders
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService
    {
        public async Task AddItemAsync(Guid orderId, string productName, decimal price, int quantity)
        {
            var order = await orderRepository.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException("Order not found");

            order.AddItem(productName, price, quantity);
            await orderRepository.UpdateAsync(order);
        }

        public async Task<Guid> Create(string email)
        {
            var order = Order.Create(email);
            await orderRepository.AddAsync(order);
            return order.Id;
        }
    }
}
