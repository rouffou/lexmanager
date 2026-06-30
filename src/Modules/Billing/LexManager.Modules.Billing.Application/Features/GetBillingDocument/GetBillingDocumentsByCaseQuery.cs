using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.GetBillingDocument;

public sealed record GetBillingDocumentsByCaseQuery(Guid CaseId, int Page = 1, int PageSize = 25)
    : IQuery<Result<PagedList<BillingDocumentSummaryResponse>>>;
