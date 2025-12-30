namespace EshopDomain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
