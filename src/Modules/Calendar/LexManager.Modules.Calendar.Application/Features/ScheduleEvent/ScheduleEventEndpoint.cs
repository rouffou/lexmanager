using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.ScheduleEvent;

public sealed class ScheduleEventEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/calendar/events", async (ScheduleEventCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/calendar/events/{id}", new { id }));
            })
            .WithName("ScheduleEvent")
            .WithTags("Calendar")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
