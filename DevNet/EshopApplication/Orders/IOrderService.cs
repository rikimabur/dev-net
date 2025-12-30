namespace EshopApplication.Orders
{
    public interface IOrderService
    {
        Task<Guid> Create(string email);
        Task AddItemAsync(Guid orderId, string productName, decimal price, int quantity);
    }
}
