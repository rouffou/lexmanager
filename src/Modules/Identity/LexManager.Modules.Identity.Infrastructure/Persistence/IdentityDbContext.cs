using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Domain.Clients;
using LexManager.SharedKernel.Domain;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Persistence boundary for the Identity module. Lives in its own <c>identity</c> schema; no other
/// module may read or write it (SRD §3.2). Publishes domain events after the transaction commits.
/// </summary>
public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options, IPublisher publisher)
    : DbContext(options), IIdentityUnitOfWork
{
    public const string Schema = "identity";

    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<AggregateRoot<ClientId>> aggregates = ChangeTracker
            .Entries<AggregateRoot<ClientId>>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<IDomainEvent> domainEvents = aggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        int result = await base.SaveChangesAsync(cancellationToken);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        foreach (AggregateRoot<ClientId> aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}
