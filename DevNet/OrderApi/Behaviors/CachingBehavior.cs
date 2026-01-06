using Microsoft.Extensions.Caching.Memory;
using OrderApi.Mediator;

namespace OrderApi.Behaviors;
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(5);

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only cache if the request implements ICacheableRequest
        if (request is not ICacheableRequest<TResponse> cacheableRequest)
        {
            return await next();
        }

        var cacheKey = cacheableRequest.CacheKey;

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse) && cachedResponse is not null)
        {
            _logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
            return cachedResponse;
        }

        _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);

        // Execute handler
        var response = await next();

        // Cache the response
        var duration = cacheableRequest.CacheDuration ?? DefaultCacheDuration;
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(duration)
            .SetSize(1);

        _cache.Set(cacheKey, response, cacheOptions);

        _logger.LogDebug("Cached response for {CacheKey} with duration {Duration}", cacheKey, duration);

        return response;
    }
}
