using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;

public sealed class RecordVerificationCheckEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/clients/due-diligence/{id:guid}/checks", async (
                Guid id,
                RecordVerificationCheckRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new RecordVerificationCheckCommand(id, body.Kind, body.Reference, body.Cleared, body.Notes), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RecordVerificationCheck")
            .WithTags("Clients — KYC/LCB-FT")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
