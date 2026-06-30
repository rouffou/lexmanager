using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.DecideDueDiligence;

public sealed class DecideDueDiligenceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients/due-diligence/{id:guid}/decision", async (
                Guid id,
                DecideDueDiligenceRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new DecideDueDiligenceCommand(id, body.Approve, body.Reason), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("DecideDueDiligence")
            .WithTags("Clients — KYC/LCB-FT")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
