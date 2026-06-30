using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed class GetBillingDocumentsByCaseQueryHandler(IBillingReadRepository readRepository)
    : IQueryHandler<GetBillingDocumentsByCaseQuery, Result<PagedList<BillingDocumentSummaryResponse>>>
{
    public async Task<Result<PagedList<BillingDocumentSummaryResponse>>> Handle(
        GetBillingDocumentsByCaseQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<BillingDocumentSummaryResponse> page =
            await readRepository.GetByCaseAsync(request.CaseId, parameters, cancellationToken);
        return Result.Success(page);
    }
}
