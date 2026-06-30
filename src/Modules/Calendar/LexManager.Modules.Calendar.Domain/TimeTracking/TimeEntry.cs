using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Calendar.Domain.TimeTracking;

/// <summary>
/// A unit of time recorded against a case (SRD Module 4: feuille de temps). Billable durations are
/// rounded up to the quarter-hour, as is customary for time-based fees.
/// </summary>
public sealed class TimeEntry : AggregateRoot<TimeEntryId>
{
    public const int RoundingMinutes = 15;

    private TimeEntry() { }

    private TimeEntry(
        TimeEntryId id,
        Guid caseId,
        Guid userId,
        string description,
        DateTime workedOnUtc,
        int durationMinutes,
        bool isBillable) : base(id)
    {
        CaseId = caseId;
        UserId = userId;
        Description = description;
        WorkedOnUtc = workedOnUtc;
        DurationMinutes = durationMinutes;
        IsBillable = isBillable;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid CaseId { get; private set; }
    public Guid UserId { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime WorkedOnUtc { get; private set; }
    public int DurationMinutes { get; private set; }
    public bool IsBillable { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static TimeEntry Log(
        Guid caseId,
        Guid userId,
        string description,
        int rawDurationMinutes,
        bool isBillable = true,
        DateTime? workedOnUtc = null)
    {
        if (caseId == Guid.Empty)
        {
            throw new BusinessRuleValidationException(CalendarErrors.TimeEntryCaseRequired);
        }

        if (rawDurationMinutes <= 0)
        {
            throw new BusinessRuleValidationException(CalendarErrors.InvalidDuration);
        }

        var entry = new TimeEntry(
            TimeEntryId.New(), caseId, userId, (description ?? string.Empty).Trim(),
            workedOnUtc ?? DateTime.UtcNow, RoundUpToQuarter(rawDurationMinutes), isBillable);

        entry.Raise(new TimeEntryLoggedDomainEvent(entry.Id.Value, caseId, entry.DurationMinutes, isBillable));
        return entry;
    }

    /// <summary>Rounds a duration up to the next quarter-hour (15 minutes).</summary>
    public static int RoundUpToQuarter(int minutes) =>
        (int)(Math.Ceiling(minutes / (double)RoundingMinutes) * RoundingMinutes);
}
