namespace EshopDomain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other) return false;
            return Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
