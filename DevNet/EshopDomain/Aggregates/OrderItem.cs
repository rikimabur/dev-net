namespace EshopDomain.Aggregates
{
    public class OrderItem
    {
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public OrderItem(string name, decimal price, int quantity)
        {
            ProductName = name;
            Price = price;
            Quantity = quantity;
        }
    }
}
