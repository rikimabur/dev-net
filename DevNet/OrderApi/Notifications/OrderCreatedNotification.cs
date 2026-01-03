using OrderApi.Mediator;

namespace OrderApi.Notifications;
public record OrderCreatedNotification(Guid OrderId, string CustomerId) : INotification;

public class SendOrderEmailHandler : INotificationHandler<OrderCreatedNotification>
{

    public SendOrderEmailHandler()
    {

    }

    public Task HandleAsync(
        OrderCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending order email for OrderId: {notification.OrderId}, CustomerId: {notification.CustomerId}");
        return Task.CompletedTask;
    }
}
public class LogOrderCreatedHandler : INotificationHandler<OrderCreatedNotification>
{
    private readonly ILogger<LogOrderCreatedHandler> _logger;

    public LogOrderCreatedHandler(ILogger<LogOrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(
        OrderCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order {OrderId} created for customer {CustomerId}",
            notification.OrderId,
            notification.CustomerId);

        await Task.CompletedTask;
    }
}