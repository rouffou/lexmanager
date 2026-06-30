using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.CloseCase;

public sealed class CloseCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/cases/{id:guid}/close", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new CloseCaseCommand(id), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("CloseCase")
            .WithTags("Cases")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
