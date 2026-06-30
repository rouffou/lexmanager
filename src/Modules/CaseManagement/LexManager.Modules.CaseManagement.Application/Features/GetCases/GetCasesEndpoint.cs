using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCases;

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
