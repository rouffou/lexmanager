using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GetCarpaAccount;

public sealed class GetCarpaAccountByCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/carpa/cases/{caseId:guid}/account", async (
                Guid caseId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<CarpaAccountResponse> result = await sender.Send(new GetCarpaAccountByCaseQuery(caseId), cancellationToken);
                return result.ToApiResult(Results.Ok);
            })
            .WithName("GetCarpaAccountByCase")
            .WithTags("Billing — CARPA")
            .Produces<CarpaAccountResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
