namespace LexManager.Modules.Calendar.Application.Abstractions;

/// <summary>Event data pushed to an external calendar.</summary>
public sealed record CalendarSyncRequest(
    string Title,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location,
    bool MaskDetails);
