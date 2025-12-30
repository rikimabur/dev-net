using EshopDomain.Events;
using MediatR;

namespace EshopInfrastructure.Events
{
    public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
    {
     
        public Task DispatchAsync(IDomainEvent domainEvent)
        {
            return mediator.Publish(domainEvent);
        }
    }
}
