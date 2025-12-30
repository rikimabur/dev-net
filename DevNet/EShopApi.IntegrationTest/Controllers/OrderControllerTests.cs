using EshopApplication.Abstractions;
using EshopDomain.Common;
using EshopInfrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace EShopApi.IntegrationTest.Controllers
{
    public class OrderControllerTests
    : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public OrderControllerTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreated_And_SavesOrder()
        {
            var email = "test@example.com";

            var response = await _client.PostAsJsonAsync("/api/order", email);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var order = await context.Orders.FirstOrDefaultAsync();
            Assert.NotNull(order);
            Assert.Equal(email.ToLower(), order.CustomerEmail.Value);
        }
    }
    public class FakeDomainEventDispatcher : IDomainEventDispatcher
    {
        public List<IDomainEvent> DispatchedEvents { get; } = new();

        public Task DispatchAsync(IDomainEvent domainEvent)
        {
            DispatchedEvents.Add(domainEvent);
            return Task.CompletedTask;
        }
    }
    public class TestWebApplicationFactory
     : WebApplicationFactory<Program>
    {
        //https://learn.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli
        //The EF Core in-memory database is not designed for performance or robustness and should not be used outside of testing environments.
        //It is not designed for production use.

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1 REMOVE EVERYTHING EF RELATED
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<AppDbContext>();

                // Remove real dispatcher
                services.RemoveAll<IDomainEventDispatcher>();

                // 2 Register ONE provider ONLY
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                // Fake dispatcher
                services.AddScoped<IDomainEventDispatcher, FakeDomainEventDispatcher>();
            });
        }
    }
}
