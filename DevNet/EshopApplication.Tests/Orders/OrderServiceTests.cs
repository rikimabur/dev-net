using EshopApplication.Orders;
using EshopDomain.Aggregates;
using EshopDomain.Repositories;
using Moq;

namespace EshopApplication.Tests.Orders
{
    public class OrderServiceTests
    {
        [Fact]
        public async Task Create_Should_Add_Order_And_ReturnId()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            Order capturedOrder = null;

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Order>(o => capturedOrder = o);

            var service = new OrderService(mockRepo.Object);
            string email = "test@example.com";

            // Act
            var orderId = await service.Create(email);

            // Assert
            Assert.NotNull(capturedOrder);
            Assert.Equal(orderId, capturedOrder.Id);
            Assert.Equal(email.ToLower(), capturedOrder.CustomerEmail.Value);

            mockRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_Should_AddItem_And_CallUpdate()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var order = Order.Create("test@example.com");

            mockRepo.Setup(r => r.GetByIdAsync(order.Id))
                    .ReturnsAsync(order);

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var service = new OrderService(mockRepo.Object);

            // Act
            await service.AddItemAsync(order.Id, "Laptop", 1200m, 1);

            // Assert
            Assert.Single(order.Items);
            var item = order.Items.First();
            Assert.Equal("Laptop", item.ProductName);
            Assert.Equal(1200m, item.Price);
            Assert.Equal(1, item.Quantity);

            mockRepo.Verify(r => r.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task AddItemAsync_Should_Throw_When_OrderNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);

            var service = new OrderService(mockRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddItemAsync(Guid.NewGuid(), "Laptop", 1000m, 1));
        }
    }
}
