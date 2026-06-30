using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetCaseTimeSummary;

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
