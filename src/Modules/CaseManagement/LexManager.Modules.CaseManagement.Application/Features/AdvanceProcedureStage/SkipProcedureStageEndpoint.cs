using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.AdvanceProcedureStage;

public sealed class SkipProcedureStageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cases/procedure/{planId:guid}/skip", async (Guid planId, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new AdvanceProcedureStageCommand(planId, Skip: true), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("SkipProcedureStage")
            .WithTags("Cases — Procédure")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
