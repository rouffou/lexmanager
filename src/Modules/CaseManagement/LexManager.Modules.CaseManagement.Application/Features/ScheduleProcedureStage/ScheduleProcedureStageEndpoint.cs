using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;

public sealed class ScheduleProcedureStageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cases/procedure/{planId:guid}/stages/{order:int}/schedule", async (
                Guid planId,
                int order,
                ScheduleProcedureStageRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new ScheduleProcedureStageCommand(planId, order, request.PlannedOnUtc);
                Result result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("ScheduleProcedureStage")
            .WithTags("Cases — Procédure")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();
    }
}
