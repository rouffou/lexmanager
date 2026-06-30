using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence;

internal sealed class CalendarEventRepository(CalendarDbContext context) : ICalendarEventRepository
{
    public Task<CalendarEvent?> GetByIdAsync(CalendarEventId id, CancellationToken cancellationToken = default) =>
        context.Events.FirstOrDefaultAsync(calendarEvent => calendarEvent.Id == id, cancellationToken);

    public async Task<IReadOnlyList<CalendarEvent>> GetOverlappingAsync(
        Guid ownerUserId,
        DateRange period,
        CancellationToken cancellationToken = default)
    {
        return await context.Events
            .AsNoTracking()
            .Where(calendarEvent =>
                calendarEvent.OwnerUserId == ownerUserId &&
                calendarEvent.Period.StartUtc < period.EndUtc &&
                period.StartUtc < calendarEvent.Period.EndUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(CalendarEvent calendarEvent) => context.Events.Add(calendarEvent);
}

internal sealed class TimeEntryRepository(CalendarDbContext context) : ITimeEntryRepository
{
    public void Add(TimeEntry timeEntry) => context.TimeEntries.Add(timeEntry);
}
