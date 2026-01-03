using FluentValidation;
using OrderApi.Mediator;
using OrderApi.Notifications;

namespace OrderApi.Commands;
public record CreateOrderCommand(string CustomerId) : IRequest<CreateOrderResponse>;
public record CreateOrderResponse(Guid OrderId);
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");
    }
}

    public class CreateOrderHandler(IMediator mediator) : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var response = new CreateOrderResponse(Guid.NewGuid());
        await mediator.PublishAsync(new OrderCreatedNotification(response.OrderId, command.CustomerId), cancellationToken);
        return await Task.FromResult(response);
    }
}
