using EshopDomain.Aggregates;
using EshopDomain.Repositories;

namespace EshopApplication.Orders
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService
    {
        public async Task<Guid> Create(string email)
        {
            var order = Order.Create(email);
            await orderRepository.AddAsync(order);
            return order.Id;
        }
    }
}
