using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using LexManager.SharedKernel.Domain;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

public sealed class CaseManagementDbContext(DbContextOptions<CaseManagementDbContext> options, IPublisher publisher)
    : DbContext(options), ICaseUnitOfWork
{
    public const string Schema = "casemanagement";

    public DbSet<Case> Cases => Set<Case>();
    public DbSet<ProcedurePlan> ProcedurePlans => Set<ProcedurePlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaseManagementDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Two aggregate types with different id shapes live here (Case, ProcedurePlan), so collect
        // events through the non-generic IHasDomainEvents view rather than AggregateRoot<CaseId>.
        List<IHasDomainEvents> aggregates = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<IDomainEvent> domainEvents = aggregates.SelectMany(aggregate => aggregate.DomainEvents).ToList();

        int result = await base.SaveChangesAsync(cancellationToken);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        foreach (IHasDomainEvents aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}
