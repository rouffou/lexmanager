using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.LogTimeEntry;

public sealed class LogTimeEntryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/time-entries", async (LogTimeEntryCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/time-entries/{id}", new { id }));
            })
            .WithName("LogTimeEntry")
            .WithTags("Time tracking")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
