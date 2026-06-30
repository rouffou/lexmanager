using System.Security.Cryptography;
using LexManager.Modules.Documents.Application.Abstractions;

namespace LexManager.Modules.Documents.Infrastructure.Storage;

/// <summary>
/// Filesystem-backed document storage for development and the free-tier deployment. The same
/// <see cref="IDocumentStorage"/> port can be implemented over cloud object storage in production
/// (SRD §5.1: encrypted storage for sensitive attachments).
/// </summary>
internal sealed class FileSystemDocumentStorage(string rootPath) : IDocumentStorage
{
    public async Task<StoredFile> SaveAsync(byte[] content, CancellationToken cancellationToken = default)
    {
        string storageKey = $"{DateTime.UtcNow:yyyy'/'MM}/{Guid.NewGuid():N}";
        string fullPath = ToFullPath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);

        string checksum = Convert.ToHexStringLower(SHA256.HashData(content));
        return new StoredFile(storageKey, content.LongLength, checksum);
    }

    public async Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default) =>
        await File.ReadAllBytesAsync(ToFullPath(storageKey), cancellationToken);

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        string fullPath = ToFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string ToFullPath(string storageKey) =>
        Path.Combine(rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
}
