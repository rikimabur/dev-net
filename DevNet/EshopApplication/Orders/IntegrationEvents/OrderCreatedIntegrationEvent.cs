namespace EshopApplication.Orders.IntegrationEvents;
/// <summary>
/// Integration event published when an order is successfully created.
/// Consumed by other services (Payment, Shipping, Inventory).
/// </summary>
public sealed record OrderCreatedIntegrationEvent
{
    public Guid OrderId { get; init; }
    public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;

    public OrderCreatedIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}