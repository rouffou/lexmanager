using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCases;

public sealed record GetCasesQuery(int Page = 1, int PageSize = 25, bool IncludeArchived = false)
    : IQuery<Result<PagedList<CaseSummaryResponse>>>;
