using EshopApplication.Abstractions;
using EshopDomain.Common;
using MediatR;

namespace EshopInfrastructure.Dispatching
{
    public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
    {
        public Task DispatchAsync(IDomainEvent domainEvent)
        {
            return mediator.Publish(domainEvent);
        }
    }
}
