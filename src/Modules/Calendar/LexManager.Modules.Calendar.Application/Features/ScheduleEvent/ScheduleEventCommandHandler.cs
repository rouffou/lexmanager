using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.ScheduleEvent;

public sealed class ScheduleEventCommandHandler(
    ICalendarEventRepository eventRepository,
    ICaseApi caseApi,
    ICalendarSyncProvider syncProvider,
    ICalendarUnitOfWork unitOfWork) : ICommandHandler<ScheduleEventCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ScheduleEventCommand request, CancellationToken cancellationToken = default)
    {
        if (request.CaseId is { } caseId && !await caseApi.CaseExistsAsync(caseId, cancellationToken))
        {
            return Result.Failure<Guid>(CalendarErrors.CaseNotFound);
        }

        DateRange period = DateRange.Create(request.StartUtc, request.EndUtc);

        if (!request.AllowOverlap)
        {
            IReadOnlyList<CalendarEvent> conflicts =
                await eventRepository.GetOverlappingAsync(request.OwnerUserId, period, cancellationToken);
            if (conflicts.Count > 0)
            {
                return Result.Failure<Guid>(CalendarErrors.ScheduleConflict);
            }
        }

        CalendarEvent calendarEvent = CalendarEvent.Schedule(
            request.OwnerUserId, request.Title, request.Type, period, request.CaseId, request.Location, request.IsPrivate);

        CalendarSyncLink? link = await syncProvider.PushAsync(
            new CalendarSyncRequest(calendarEvent.Title, period.StartUtc, period.EndUtc, calendarEvent.Location, calendarEvent.IsPrivate),
            cancellationToken);

        if (link is not null)
        {
            calendarEvent.LinkExternal(link.Provider, link.ExternalId, link.ETag);
        }

        eventRepository.Add(calendarEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(calendarEvent.Id.Value);
    }
}
