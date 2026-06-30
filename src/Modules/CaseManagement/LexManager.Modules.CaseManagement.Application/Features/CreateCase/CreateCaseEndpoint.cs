using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.CaseManagement.Application.Features.CreateCase;

public sealed class CreateCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/cases", async (CreateCaseCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/cases/{id}", new { id }));
            })
            .WithName("CreateCase")
            .WithTags("Cases")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
