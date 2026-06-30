namespace LexManager.Modules.Documents.Contracts;

public sealed record DocumentSummaryResponse(
    Guid Id,
    Guid CaseId,
    string FileName,
    string Category,
    int CurrentVersion,
    DateTime CreatedOnUtc);
