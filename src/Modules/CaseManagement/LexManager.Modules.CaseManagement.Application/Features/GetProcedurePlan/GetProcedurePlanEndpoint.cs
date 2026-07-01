using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GetProcedurePlan;

public sealed class GetProcedurePlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/cases/{caseId:guid}/procedure", async (Guid caseId, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<ProcedurePlanResponse> result = await sender.Send(new GetProcedurePlanQuery(caseId), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetProcedurePlan")
            .WithTags("Cases — Procédure")
            .Produces<ProcedurePlanResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
