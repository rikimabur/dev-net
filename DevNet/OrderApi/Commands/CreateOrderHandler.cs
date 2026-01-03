using OrderApi.Mediator;

namespace OrderApi.Commands;
public record CreateOrderCommand(string CustomerId) : IRequest<CreateOrderResponse>;
public record CreateOrderResponse(Guid OrderId);

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateOrderResponse(Guid.NewGuid());
        return await Task.FromResult(response);
    }
}
