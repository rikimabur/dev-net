using EshopDomain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace EshopInfrastructure.Persistence;
 public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.OwnsOne(x => x.CustomerEmail, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("Email")
                     .IsRequired();
            });
            builder.OwnsMany(typeof(OrderItem), "_items", b =>
            {
                b.WithOwner().HasForeignKey("OrderId");
                b.Property<Guid>("Id");
                b.HasKey("Id");
            });
        });
    }
}