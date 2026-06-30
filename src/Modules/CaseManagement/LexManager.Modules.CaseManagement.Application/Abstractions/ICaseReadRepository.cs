using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.CaseManagement.Contracts;

namespace LexManager.Modules.CaseManagement.Application.Abstractions;

/// <summary>Read-side port returning flat DTOs (CQRS). Archived cases are excluded by the
/// DbContext's global query filter unless explicitly included (SRD §5.3).</summary>
public interface ICaseReadRepository
{
    Task<CaseResponse?> GetByIdAsync(Guid caseId, CancellationToken cancellationToken = default);

    Task<PagedList<CaseSummaryResponse>> GetPagedAsync(
        PaginationParameters parameters,
        bool includeArchived,
        CancellationToken cancellationToken = default);
}
