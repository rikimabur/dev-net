
using EshopDomain.Common;

namespace EshopApplication.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}
