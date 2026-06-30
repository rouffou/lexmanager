namespace LexManager.Modules.Calendar.Contracts;

public sealed record TimeEntryResponse(
    Guid Id,
    Guid CaseId,
    Guid UserId,
    string Description,
    DateTime WorkedOnUtc,
    int DurationMinutes,
    bool IsBillable);
