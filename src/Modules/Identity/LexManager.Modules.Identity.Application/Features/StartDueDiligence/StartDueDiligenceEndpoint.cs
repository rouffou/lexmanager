using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.StartDueDiligence;

public sealed class StartDueDiligenceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients/{clientId:guid}/due-diligence", async (
                Guid clientId,
                StartDueDiligenceRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(
                    new StartDueDiligenceCommand(clientId, body.RiskLevel, body.IsPoliticallyExposed), cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/clients/due-diligence/{id}", new { id }));
            })
            .WithName("StartDueDiligence")
            .WithTags("Clients — KYC/LCB-FT")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
