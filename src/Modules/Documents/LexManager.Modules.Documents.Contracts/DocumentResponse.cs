namespace LexManager.Modules.Documents.Contracts;

public sealed record DocumentResponse(
    Guid Id,
    Guid CaseId,
    string FileName,
    string ContentType,
    string Category,
    bool IsConfidential,
    int CurrentVersion,
    IReadOnlyList<DocumentVersionResponse> Versions,
    DateTime CreatedOnUtc);
