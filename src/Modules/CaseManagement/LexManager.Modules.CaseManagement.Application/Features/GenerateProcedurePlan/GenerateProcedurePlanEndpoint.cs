using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;

public sealed class GenerateProcedurePlanEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cases/{caseId:guid}/procedure", async (
                Guid caseId,
                GenerateProcedurePlanRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new GenerateProcedurePlanCommand(caseId, request.ProcedureType, request.ReferenceOnUtc);
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/cases/{caseId}/procedure", new { id }));
            })
            .WithName("GenerateProcedurePlan")
            .WithTags("Cases — Procédure")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }
}
