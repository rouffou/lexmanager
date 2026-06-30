namespace LexManager.Modules.CaseManagement.Contracts;

public sealed record CaseSummaryResponse(
    Guid Id,
    string Title,
    Guid ClientId,
    string Status,
    DateTime OpenedOnUtc);
