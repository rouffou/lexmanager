using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Billing.Contracts;

namespace LexManager.Modules.Billing.Application.Abstractions;

/// <summary>Read-side port (CQRS) returning flat DTOs.</summary>
public interface IBillingReadRepository
{
    Task<BillingDocumentResponse?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<PagedList<BillingDocumentSummaryResponse>> GetByCaseAsync(
        Guid caseId,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
