using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCases;

public sealed class GetCasesQueryHandler(ICaseReadRepository readRepository)
    : IQueryHandler<GetCasesQuery, Result<PagedList<CaseSummaryResponse>>>
{
    public async Task<Result<PagedList<CaseSummaryResponse>>> Handle(
        GetCasesQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<CaseSummaryResponse> page =
            await readRepository.GetPagedAsync(parameters, request.IncludeArchived, cancellationToken);

        return Result.Success(page);
    }
}
