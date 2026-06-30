using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetCaseTimeSummary;

public sealed record GetCaseTimeSummaryQuery(Guid CaseId) : IQuery<Result<CaseTimeSummaryResponse>>;

public sealed class GetCaseTimeSummaryQueryHandler(ICalendarReadRepository readRepository)
    : IQueryHandler<GetCaseTimeSummaryQuery, Result<CaseTimeSummaryResponse>>
{
    public async Task<Result<CaseTimeSummaryResponse>> Handle(GetCaseTimeSummaryQuery request, CancellationToken cancellationToken = default)
    {
        CaseTimeSummaryResponse summary = await readRepository.GetCaseTimeSummaryAsync(request.CaseId, cancellationToken);
        return Result.Success(summary);
    }
}

public sealed class GetCaseTimeSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/time-entries/summary", async (Guid caseId, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<CaseTimeSummaryResponse> result = await sender.Send(new GetCaseTimeSummaryQuery(caseId), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCaseTimeSummary")
            .WithTags("Time tracking")
            .Produces<CaseTimeSummaryResponse>();
    }
}
