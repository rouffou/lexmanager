namespace LexManager.Modules.Documents.Contracts;

public sealed record DocumentVersionResponse(int VersionNumber, long SizeBytes, string Checksum, DateTime UploadedOnUtc);
