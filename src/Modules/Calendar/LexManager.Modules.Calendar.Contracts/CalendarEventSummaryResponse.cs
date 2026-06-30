namespace LexManager.Modules.Calendar.Contracts;

public sealed record CalendarEventSummaryResponse(
    Guid Id,
    string Title,
    string Type,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsPrivate);
