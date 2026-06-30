using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed class RescheduleEventEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/calendar/events/{id:guid}/reschedule", async (
                Guid id,
                RescheduleEventRequest body,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result result = await sender.Send(new RescheduleEventCommand(id, body.StartUtc, body.EndUtc), cancellationToken);
                return result.ToApiResult(() => Results.NoContent());
            })
            .WithName("RescheduleEvent")
            .WithTags("Calendar")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
