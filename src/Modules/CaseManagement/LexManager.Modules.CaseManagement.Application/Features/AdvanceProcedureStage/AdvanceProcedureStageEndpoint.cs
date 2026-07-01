using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.AdvanceProcedureStage;

public sealed class AdvanceProcedureStageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cases/procedure/{planId:guid}/advance", async (Guid planId, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new AdvanceProcedureStageCommand(planId), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("AdvanceProcedureStage")
            .WithTags("Cases — Procédure")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
