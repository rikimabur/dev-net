using Microsoft.Extensions.Caching.Memory;
using OrderApi.Mediator;

namespace OrderApi.Queries
{
    public record GetOrderQuery(Guid OrderId) : IRequest<GetOrderResponse>;
    public record class GetOrderResponse(Guid OrderId, decimal Total);
    public record GetOrderQueryCacheable(string OrderId) : ICacheableRequest<GetOrderResponse>
    {
        public string CacheKey => $"order:{OrderId}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);
    }

    public class GetOrderHandler : IRequestHandler<GetOrderQueryCacheable, GetOrderResponse>
    {
        public async Task<GetOrderResponse> HandleAsync(GetOrderQueryCacheable request, CancellationToken cancellationToken)
        {
            var response = new GetOrderResponse(Guid.NewGuid(), 100.00m);
            return await Task.FromResult(response);
        }
    }
}
