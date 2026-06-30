using System.Collections.Concurrent;
using LexManager.Modules.Documents.Application.Abstractions;

namespace LexManager.Modules.Documents.IntegrationTests.Infrastructure;

/// <summary>In-memory <see cref="IDocumentStorage"/> so integration tests don't touch the filesystem.</summary>
internal sealed class InMemoryDocumentStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<string, byte[]> _files = new();

    public Task<StoredFile> SaveAsync(byte[] content, CancellationToken cancellationToken = default)
    {
        string key = Guid.NewGuid().ToString("N");
        _files[key] = content;
        string checksum = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(content));
        return Task.FromResult(new StoredFile(key, content.LongLength, checksum));
    }

    public Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(_files.TryGetValue(storageKey, out byte[]? content) ? content : []);

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        _files.TryRemove(storageKey, out _);
        return Task.CompletedTask;
    }
}
