using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetEventById;

public sealed record GetEventByIdQuery(Guid EventId) : IQuery<Result<CalendarEventResponse>>;

public sealed class GetEventByIdQueryHandler(ICalendarReadRepository readRepository)
    : IQueryHandler<GetEventByIdQuery, Result<CalendarEventResponse>>
{
    public async Task<Result<CalendarEventResponse>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken = default)
    {
        CalendarEventResponse? calendarEvent = await readRepository.GetEventByIdAsync(request.EventId, cancellationToken);

        return calendarEvent is null
            ? Result.Failure<CalendarEventResponse>(CalendarErrors.EventNotFound)
            : Result.Success(calendarEvent);
    }
}

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
