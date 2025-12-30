using EshopDomain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EshopInfrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Primary Key
            builder.HasKey(o => o.Id);

            // Email Value Object
            builder.OwnsOne(o => o.CustomerEmail, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired();
            });

            // Order Items Collection
            builder.OwnsMany(typeof(OrderItem), "_items", b =>
            {
                b.WithOwner().HasForeignKey("OrderId");
                b.Property<Guid>("Id");
                b.HasKey("Id");
            });

            builder.Ignore(o => o.DomainEvents);
        }
    }
}
