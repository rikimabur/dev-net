namespace EshopApplication.Orders
{
    public interface IOrderService
    {
        Task<Guid> Create(string email);
    }
}
