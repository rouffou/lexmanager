using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetEventById;

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
