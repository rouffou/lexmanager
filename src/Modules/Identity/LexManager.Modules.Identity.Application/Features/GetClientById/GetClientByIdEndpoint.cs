using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Identity.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Identity.Application.Features.GetClientById;

public sealed class GetClientByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/clients/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<ClientResponse> result = await sender.Send(new GetClientByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetClientById")
            .WithTags("Clients")
            .Produces<ClientResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
