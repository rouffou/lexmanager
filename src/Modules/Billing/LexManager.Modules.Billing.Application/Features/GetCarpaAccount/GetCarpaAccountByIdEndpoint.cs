using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Billing.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Billing.Application.Features.GetCarpaAccount;

public sealed class GetCarpaAccountByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/billing/carpa/accounts/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<CarpaAccountResponse> result = await sender.Send(new GetCarpaAccountByIdQuery(id), cancellationToken);
                return result.ToApiResult(Results.Ok);
            })
            .WithName("GetCarpaAccountById")
            .WithTags("Billing — CARPA")
            .Produces<CarpaAccountResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
