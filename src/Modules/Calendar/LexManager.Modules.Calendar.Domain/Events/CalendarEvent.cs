using LexManager.Modules.Calendar.Domain.Common;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.Calendar.Domain.Events;

/// <summary>
/// An entry in the shared legal agenda (hearing, deliberation, client appointment, procedure
/// deadline). May be linked to a case and synchronised with an external calendar; when private,
/// its details are masked outside the firm (SRD Module 4 confidentiality).
/// </summary>
public sealed class CalendarEvent : AggregateRoot<CalendarEventId>
{
    private CalendarEvent() { }

    private CalendarEvent(
        CalendarEventId id,
        Guid ownerUserId,
        string title,
        CalendarEventType type,
        DateRange period,
        Guid? caseId,
        string? location,
        bool isPrivate) : base(id)
    {
        OwnerUserId = ownerUserId;
        Title = title;
        Type = type;
        Period = period;
        CaseId = caseId;
        Location = location;
        IsPrivate = isPrivate;
        Provider = CalendarProvider.None;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid OwnerUserId { get; private set; }
    public string Title { get; private set; } = null!;
    public CalendarEventType Type { get; private set; }
    public DateRange Period { get; private set; } = null!;
    public Guid? CaseId { get; private set; }
    public string? Location { get; private set; }
    public bool IsPrivate { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    // External synchronisation (Microsoft Graph / Google Calendar).
    public CalendarProvider Provider { get; private set; }
    public string? ExternalId { get; private set; }
    public string? ExternalETag { get; private set; }

    public static CalendarEvent Schedule(
        Guid ownerUserId,
        string title,
        CalendarEventType type,
        DateRange period,
        Guid? caseId = null,
        string? location = null,
        bool isPrivate = false)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleValidationException(CalendarErrors.EmptyTitle);
        }

        var calendarEvent = new CalendarEvent(
            CalendarEventId.New(), ownerUserId, title.Trim(), type, period, caseId,
            string.IsNullOrWhiteSpace(location) ? null : location.Trim(), isPrivate);

        calendarEvent.Raise(new CalendarEventScheduledDomainEvent(
            calendarEvent.Id.Value, ownerUserId, period.StartUtc, period.EndUtc));
        return calendarEvent;
    }

    public void Reschedule(DateRange period) => Period = period;

    public void MarkPrivate() => IsPrivate = true;

    /// <summary>Records the link to an external calendar after a successful sync push.</summary>
    public void LinkExternal(CalendarProvider provider, string externalId, string? etag)
    {
        Provider = provider;
        ExternalId = externalId;
        ExternalETag = etag;
    }

    public bool Overlaps(CalendarEvent other) => Period.Overlaps(other.Period);
}
