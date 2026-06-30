using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Contracts;
using Mediarq.UnitOfWork;

namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Module-scoped unit of work (Mediarq's <see cref="IUnitOfWork"/>) for Documents.</summary>
public interface IDocumentUnitOfWork : IUnitOfWork;

/// <summary>Metadata returned after persisting a blob to the storage backend.</summary>
public sealed record StoredFile(string StorageKey, long SizeBytes, string Checksum);

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

/// <summary>Result of rendering a document template (mail merge / publipostage, SRD Module 3).</summary>
public sealed record RenderedTemplate(string FileName, string ContentType, byte[] Content);

/// <summary>Renders a named template against a set of field values into a document.</summary>
public interface ITemplateRenderer
{
    Task<RenderedTemplate?> RenderAsync(
        string templateKey,
        IReadOnlyDictionary<string, string> values,
        CancellationToken cancellationToken = default);
}

/// <summary>Pointer to a stored document version's bytes plus the metadata needed to serve it.</summary>
public sealed record DocumentFileRef(string FileName, string ContentType, string StorageKey);

/// <summary>Read-side port (CQRS) returning flat DTOs.</summary>
public interface IDocumentReadRepository
{
    Task<DocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PagedList<DocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    Task<DocumentFileRef?> GetFileRefAsync(Guid documentId, int? versionNumber, CancellationToken cancellationToken = default);

    Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
