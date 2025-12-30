using EshopDomain.Aggregates;

namespace EshopDomain.Tests
{
    public class Order
    {
        [Fact]
        public void Order_ShouldCalculateTotalCorrectly()
        {
            var order = Aggregates.Order.Create("test@mail.com");
            order.AddItem("Test", 10, 2);
            var total = order.Total();
            Assert.Equal(20, total);
        }
    }
}
