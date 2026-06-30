namespace LexManager.Modules.Documents.Domain.Documents;

/// <summary>
/// One immutable revision of a document's binary content (SRD Module 3: versionnage). The actual
/// bytes live in the storage backend; this records the pointer (storage key) and integrity data.
/// </summary>
public sealed class DocumentVersion
{
    private DocumentVersion() { }

    internal DocumentVersion(int versionNumber, string storageKey, long sizeBytes, string checksum)
    {
        VersionNumber = versionNumber;
        StorageKey = storageKey;
        SizeBytes = sizeBytes;
        Checksum = checksum;
        UploadedOnUtc = DateTime.UtcNow;
    }

    public int VersionNumber { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string Checksum { get; private set; } = null!;
    public DateTime UploadedOnUtc { get; private set; }
}
