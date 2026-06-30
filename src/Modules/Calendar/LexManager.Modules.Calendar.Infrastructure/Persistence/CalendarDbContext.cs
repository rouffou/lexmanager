using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using LexManager.SharedKernel.Domain;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence;

/// <summary>
/// Persistence boundary for the Calendar module — holds two aggregates (events and time entries)
/// in the <c>calendar</c> schema. Domain events from either aggregate are dispatched after commit
/// via the non-generic <see cref="IHasDomainEvents"/> contract.
/// </summary>
public sealed class CalendarDbContext(DbContextOptions<CalendarDbContext> options, IPublisher publisher)
    : DbContext(options), ICalendarUnitOfWork
{
    public const string Schema = "calendar";

    public DbSet<CalendarEvent> Events => Set<CalendarEvent>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalendarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
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
