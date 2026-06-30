using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.TimeTracking;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence;

internal sealed class TimeEntryRepository(CalendarDbContext context) : ITimeEntryRepository
{
    public void Add(TimeEntry timeEntry) => context.TimeEntries.Add(timeEntry);
}
