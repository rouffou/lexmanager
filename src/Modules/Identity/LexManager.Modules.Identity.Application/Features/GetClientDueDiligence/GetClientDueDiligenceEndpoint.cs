using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.GetClientDueDiligence;

public sealed class GetClientDueDiligenceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients/{clientId:guid}/due-diligence", async (
                Guid clientId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<DueDiligenceResponse> result = await sender.Send(new GetClientDueDiligenceQuery(clientId), cancellationToken);
                return result.ToApiResult(Results.Ok);
            })
            .WithName("GetClientDueDiligence")
            .WithTags("Clients — KYC/LCB-FT")
            .Produces<DueDiligenceResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
