using OrderApi.Mediator;

namespace OrderApi.Queries
{
    public record GetOrderQuery(Guid OrderId) : IRequest<GetOrderResponse>;
    public record class GetOrderResponse(Guid OrderId, decimal Total);
    public class GetOrderHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
    {
        public async Task<GetOrderResponse> HandleAsync(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var response = new GetOrderResponse(Guid.NewGuid(), 100.00m);
            return await Task.FromResult(response);
        }
    }
}
