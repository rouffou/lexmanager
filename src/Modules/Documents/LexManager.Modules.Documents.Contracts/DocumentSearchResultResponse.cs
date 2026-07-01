namespace LexManager.Modules.Documents.Contracts;

/// <summary>
/// A single full-text search hit: the document identity plus a short highlighted excerpt of the
/// matching OCR-extracted body (SRD §7.2).
/// </summary>
public sealed record DocumentSearchResultResponse(
    Guid Id,
    Guid CaseId,
    string FileName,
    string Category,
    string? Highlight,
    DateTime CreatedOnUtc);
