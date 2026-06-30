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

    Task<DocumentFileRef?> GetFileRefAsync(Guid documentId, int? versionNumber, CancellationToken cancellationToken = default);

    Task<int> CountByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
