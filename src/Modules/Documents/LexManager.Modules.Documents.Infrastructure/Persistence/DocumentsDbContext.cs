using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Domain.Documents;
using LexManager.SharedKernel.Domain;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Documents.Infrastructure.Persistence;

public sealed class DocumentsDbContext(DbContextOptions<DocumentsDbContext> options, IPublisher publisher)
    : DbContext(options), IDocumentUnitOfWork
{
    public const string Schema = "documents";

    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<AggregateRoot<DocumentId>> aggregates = ChangeTracker
            .Entries<AggregateRoot<DocumentId>>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        List<IDomainEvent> domainEvents = aggregates.SelectMany(aggregate => aggregate.DomainEvents).ToList();

        int result = await base.SaveChangesAsync(cancellationToken);

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        foreach (AggregateRoot<DocumentId> aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return result;
    }
}
