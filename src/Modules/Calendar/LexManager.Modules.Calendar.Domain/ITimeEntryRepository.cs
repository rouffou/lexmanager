using LexManager.Modules.Calendar.Domain.TimeTracking;

namespace LexManager.Modules.Calendar.Domain;

/// <summary>Write-side port for the <see cref="TimeEntry"/> aggregate.</summary>
public interface ITimeEntryRepository
{
    void Add(TimeEntry timeEntry);
}
