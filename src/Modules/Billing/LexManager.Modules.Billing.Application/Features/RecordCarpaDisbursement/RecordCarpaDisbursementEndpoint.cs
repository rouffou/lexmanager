using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;

public sealed class RecordCarpaDisbursementEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/carpa/accounts/{id:guid}/disbursements", async (
                Guid id,
                RecordCarpaDisbursementRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new RecordCarpaDisbursementCommand(id, body.Amount, body.Description, body.Counterparty), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RecordCarpaDisbursement")
            .WithTags("Billing — CARPA")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
