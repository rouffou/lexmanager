using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;

public sealed class RecordCarpaDepositEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/billing/carpa/accounts/{id:guid}/deposits", async (
                Guid id,
                RecordCarpaDepositRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(
                    new RecordCarpaDepositCommand(id, body.Amount, body.Description, body.Counterparty), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RecordCarpaDeposit")
            .WithTags("Billing — CARPA")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
