using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace OrderApi.Mediator;
public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, object> _handlerInvokerCache;
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _handlerInvokerCache = new ConcurrentDictionary<Type, object>();
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerInvoker = (Func<IRequest<TResponse>, IServiceProvider, CancellationToken, Task<TResponse>>)
                  _handlerInvokerCache.GetOrAdd(requestType, rt =>
                  {
                      return CreateHandlerInvoker<TResponse>(rt);
                  });

        return await handlerInvoker(request, _serviceProvider, cancellationToken);

    }
    private Func<object, object, CancellationToken, Task<TResponse>>CreateCompiledInvoker<TResponse>(Type requestType)
    {
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handleMethod = handlerType.GetMethod("HandleAsync")!;

        // Parameters for our delegate
        var handlerParam = Expression.Parameter(typeof(object), "handler");
        var requestParam = Expression.Parameter(typeof(object), "request");
        var tokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        // Cast the parameters to their actual types
        var handlerCast = Expression.Convert(handlerParam, handlerType);
        var requestCast = Expression.Convert(requestParam, requestType);

        // Build the method call: ((THandler)handler).HandleAsync((TRequest)request, cancellationToken)
        var methodCall = Expression.Call(handlerCast, handleMethod, requestCast, tokenParam);

        // Compile to a delegate
        var lambda = Expression.Lambda<Func<object, object, CancellationToken, Task<TResponse>>>(
            methodCall,
            handlerParam,
            requestParam,
            tokenParam);

        return lambda.Compile();
    }

    private Func<IRequest<TResponse>, IServiceProvider, CancellationToken, Task<TResponse>>
    CreateHandlerInvoker<TResponse>(Type requestType)
    {
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        // Create the compiled invoker once
        var compiledInvoker = CreateCompiledInvoker<TResponse>(requestType);

        return (request, serviceProvider, cancellationToken) =>
        {
            var handler = serviceProvider.GetService(handlerType);
            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"No handler registered for request type {requestType.Name}");
            }

            // Direct delegate call, no reflection
            return compiledInvoker(handler, request, cancellationToken);
        };
    }
}
