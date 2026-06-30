namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>
/// Secure binary storage for document content (SRD Module 3). The implementation may target the
/// local filesystem (dev) or cloud object storage (prod); the domain only ever sees storage keys.
/// </summary>
public interface IDocumentStorage
{
    Task<StoredFile> SaveAsync(byte[] content, CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
