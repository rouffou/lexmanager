namespace LexManager.Modules.Calendar.Contracts;

public sealed record CalendarEventResponse(
    Guid Id,
    Guid OwnerUserId,
    string Title,
    string Type,
    Guid? CaseId,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location,
    bool IsPrivate,
    string Provider);
