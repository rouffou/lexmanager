using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Domain.TimeTracking;

namespace LexManager.Modules.Calendar.Domain;

/// <summary>Write-side port for the <see cref="CalendarEvent"/> aggregate.</summary>
public interface ICalendarEventRepository
{
    Task<CalendarEvent?> GetByIdAsync(CalendarEventId id, CancellationToken cancellationToken = default);

    /// <summary>Events owned by a user that overlap the given range — used for conflict detection.</summary>
    Task<IReadOnlyList<CalendarEvent>> GetOverlappingAsync(
        Guid ownerUserId,
        DateRange period,
        CancellationToken cancellationToken = default);

    void Add(CalendarEvent calendarEvent);
}

/// <summary>Write-side port for the <see cref="TimeEntry"/> aggregate.</summary>
public interface ITimeEntryRepository
{
    void Add(TimeEntry timeEntry);
}
