namespace LexManager.Modules.Documents.Contracts;

public sealed record DocumentVersionResponse(int VersionNumber, long SizeBytes, string Checksum, DateTime UploadedOnUtc);

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

public sealed record DocumentSummaryResponse(
    Guid Id,
    Guid CaseId,
    string FileName,
    string Category,
    int CurrentVersion,
    DateTime CreatedOnUtc);

/// <summary>File payload for download responses (content streamed by the endpoint).</summary>
public sealed record DocumentDownload(string FileName, string ContentType, byte[] Content);
