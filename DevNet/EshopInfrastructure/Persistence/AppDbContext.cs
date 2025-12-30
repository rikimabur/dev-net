using EshopApplication.Abstractions;
using EshopDomain.Aggregates;
using EshopDomain.Common;
using EshopInfrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EshopInfrastructure.Persistence;
public class AppDbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DbSet<Order> Orders { get; private set; } = default!;
    public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Collect all entities with domain events
        var entitiesWithEvents = ChangeTracker
            .Entries<AggregateRoot>()   // Prefer AggregateRoot instead of Entity if only aggregates raise events
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        // 2. Collect all domain events
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // 3. Save changes first
        var result = await base.SaveChangesAsync(cancellationToken);

        // 4. Dispatch events
        foreach (var domainEvent in domainEvents)
        {
            await _dispatcher.DispatchAsync(domainEvent);
        }

        // 5. Clear events to avoid duplicate dispatching
        entitiesWithEvents.ForEach(e => e.ClearEvents());

        return result;
    }
}