using EshopDomain.Entities;

namespace EshopDomain.Common
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void AddEvent(IDomainEvent @event) => _domainEvents.Add(@event);
        public void ClearEvents() => _domainEvents.Clear();
    }
}
