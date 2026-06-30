namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Metadata returned after persisting a blob to the storage backend.</summary>
public sealed record StoredFile(string StorageKey, long SizeBytes, string Checksum);
