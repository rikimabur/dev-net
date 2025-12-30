using EshopDomain.Aggregates;
using EshopDomain.Repositories;
using EshopInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EshopInfrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            // eager load _items collection
            return await _context.Orders.Include("_items").FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
