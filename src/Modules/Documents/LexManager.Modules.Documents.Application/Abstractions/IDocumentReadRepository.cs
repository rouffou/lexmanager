using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Documents.Contracts;

namespace LexManager.Modules.Documents.Application.Abstractions;

/// <summary>Read-side port (CQRS) returning flat DTOs.</summary>
public interface IDocumentReadRepository
{
    Task<DocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PagedList<DocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Full-text search over document file names and OCR-extracted bodies (SRD §7.2). Optionally
    /// scoped to a single case. <paramref name="term"/> is natural user input (words, quoted
    /// phrases, <c>-exclusions</c>).
    /// </summary>
    Task<PagedList<DocumentSearchResultResponse>> SearchAsync(
        string term,
        Guid? caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    Task<DocumentFileRef?> GetFileRefAsync(Guid documentId, int? versionNumber, CancellationToken cancellationToken = default);

    Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
