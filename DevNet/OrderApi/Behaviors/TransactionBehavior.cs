//using OrderApi.Mediator;
//using System;
//using System.Transactions;

//namespace OrderApi.Behaviors;
//public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    private readonly AppDbContext _dbContext;
//    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

//    public TransactionBehavior(AppDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
//    {
//        _dbContext = dbContext;
//        _logger = logger;
//    }

//    public async Task<TResponse> HandleAsync(
//        TRequest request,
//        RequestHandlerDelegate<TResponse> next,
//        CancellationToken cancellationToken)
//    {

//        #region TransactionScope
//        //TransactionScope is more flexible since it works across multiple DbContexts and even non-EF data sources.
//        //But it's also more complex and has gotchas with async code (hence TransactionScopeAsyncFlowOption.Enabled).
//       // using var scope = new TransactionScope(
//       //TransactionScopeOption.Required,
//       //new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
//       //TransactionScopeAsyncFlowOption.Enabled);

//       // var response = await next();

//       // scope.Complete();
//        #endregion
//        // Skip for queries (we'll mark them with an interface)
//        if (request is IQuery<TResponse>)
//        {
//            return await next();
//        }

//        var requestName = typeof(TRequest).Name;

//        // If there's already a transaction, don't start a new one
//        if (_dbContext.Database.CurrentTransaction is not null)
//        {
//            _logger.LogDebug("Using existing transaction for {RequestName}", requestName);
//            return await next();
//        }

//        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

//        try
//        {
//            _logger.LogDebug("Starting transaction for {RequestName}", requestName);

//            var response = await next();

//            await _dbContext.SaveChangesAsync(cancellationToken);
//            await transaction.CommitAsync(cancellationToken);

//            _logger.LogDebug("Committed transaction for {RequestName}", requestName);

//            return response;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Rolling back transaction for {RequestName}", requestName);
//            await transaction.RollbackAsync(cancellationToken);
//            throw;
//        }
//    }
//}
