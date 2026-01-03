using OrderApi.Behaviors;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace OrderApi.Mediator;
public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
    Task PublishAsync<TNotification>(
    TNotification notification,
    CancellationToken cancellationToken = default)
    where TNotification : INotification;
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

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
        {
            var handleMethod = handlerType.GetMethod("HandleAsync")!;
            return (Task)handleMethod.Invoke(handler, [notification, cancellationToken])!;
        });

        await Task.WhenAll(tasks);
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        //var handlerInvoker = (Func<IRequest<TResponse>, IServiceProvider, CancellationToken, Task<TResponse>>)
        //          _handlerInvokerCache.GetOrAdd(requestType, rt =>
        //          {
        //              return CreateHandlerInvoker<TResponse>(rt);
        //          });

        //return await handlerInvoker(request, _serviceProvider, cancellationToken);


        var responseType = typeof(TResponse);

        // Get the handler
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"No handler registered for request type {requestType.Name}");

        // Get all pipeline behaviors for this request type
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider
            .GetServices(behaviorType)
            .Cast<object>()
            .Reverse()
            .ToList();

        // Build the pipeline
        var handleMethod = handlerType.GetMethod("HandleAsync")!;
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var task = (Task<TResponse>)handleMethod.Invoke(
                handler,
                [request, cancellationToken])!;
            return task;
        };

        // Wrap with behaviors (in reverse order so first registered runs first)
        foreach (var behavior in behaviors)
        {
            var currentPipeline = pipeline;
            var behaviorHandleMethod = behaviorType.GetMethod("HandleAsync")!;

            pipeline = () =>
            {
                var task = (Task<TResponse>)behaviorHandleMethod.Invoke(
                    behavior,
                    [request, currentPipeline, cancellationToken])!;
                return task;
            };
        }

        return await pipeline();
    }
    private Func<object, object, CancellationToken, Task<TResponse>> CreateCompiledInvoker<TResponse>(Type requestType)
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

//Without using ConcurrentDictionary
public class MediatorV2 : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, object> _handlerInvokerCache;
    private readonly object _lock = new();
    public MediatorV2(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _handlerInvokerCache = [];
    }

    public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        throw new NotImplementedException();
    }

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        Func<IRequest<TResponse>, IServiceProvider, CancellationToken, Task<TResponse>> handlerInvoker;

        if (!_handlerInvokerCache.TryGetValue(requestType, out var cached))
        {
            lock (_lock)
            {
                if (!_handlerInvokerCache.TryGetValue(requestType, out cached))
                {
                    cached = CreateHandlerInvoker<TResponse>(requestType);
                    _handlerInvokerCache[requestType] = cached;
                }
            }
        }

        handlerInvoker = (Func<IRequest<TResponse>, IServiceProvider, CancellationToken, Task<TResponse>>)cached;
        return await handlerInvoker(request, _serviceProvider, cancellationToken);
    }
    private Func<object, object, CancellationToken, Task<TResponse>> CreateCompiledInvoker<TResponse>(Type requestType)
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
