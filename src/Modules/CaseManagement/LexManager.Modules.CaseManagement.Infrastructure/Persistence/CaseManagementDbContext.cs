using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.SharedKernel.Domain;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

public sealed class CaseManagementDbContext(DbContextOptions<CaseManagementDbContext> options, IPublisher publisher)
    : DbContext(options), ICaseUnitOfWork
{
    public const string Schema = "casemanagement";

    public DbSet<Case> Cases => Set<Case>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaseManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<AggregateRoot<CaseId>> aggregates = ChangeTracker
            .Entries<AggregateRoot<CaseId>>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<IDomainEvent> domainEvents = aggregates.SelectMany(aggregate => aggregate.DomainEvents).ToList();

        int result = await base.SaveChangesAsync(cancellationToken);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        foreach (AggregateRoot<CaseId> aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}
