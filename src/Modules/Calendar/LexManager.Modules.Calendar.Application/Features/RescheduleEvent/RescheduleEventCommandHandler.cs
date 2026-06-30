using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed class RescheduleEventCommandHandler(
    ICalendarEventRepository eventRepository,
    ICalendarUnitOfWork unitOfWork) : ICommandHandler<RescheduleEventCommand, Result>
{
    public async Task<Result> Handle(RescheduleEventCommand request, CancellationToken cancellationToken = default)
    {
        CalendarEvent? calendarEvent = await eventRepository.GetByIdAsync(new CalendarEventId(request.EventId), cancellationToken);
        if (calendarEvent is null)
        {
            return Result.Failure(CalendarErrors.EventNotFound);
        }

        calendarEvent.Reschedule(DateRange.Create(request.StartUtc, request.EndUtc));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
