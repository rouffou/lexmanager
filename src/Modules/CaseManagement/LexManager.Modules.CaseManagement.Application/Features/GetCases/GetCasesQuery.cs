using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCases;

public sealed record GetCasesQuery(int Page = 1, int PageSize = 25, bool IncludeArchived = false)
    : IQuery<Result<PagedList<CaseSummaryResponse>>>;

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

public sealed class GetCasesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/cases", async (
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 25,
                bool includeArchived = false) =>
            {
                Result<PagedList<CaseSummaryResponse>> result =
                    await sender.Send(new GetCasesQuery(page, pageSize, includeArchived), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCases")
            .WithTags("Cases")
            .Produces<PagedList<CaseSummaryResponse>>();
    }
}
