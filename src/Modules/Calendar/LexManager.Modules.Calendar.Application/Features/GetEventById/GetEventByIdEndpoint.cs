using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetEventById;

public sealed class GetEventByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/calendar/events/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<CalendarEventResponse> result = await sender.Send(new GetEventByIdQuery(id), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetCalendarEventById")
            .WithTags("Calendar")
            .Produces<CalendarEventResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
