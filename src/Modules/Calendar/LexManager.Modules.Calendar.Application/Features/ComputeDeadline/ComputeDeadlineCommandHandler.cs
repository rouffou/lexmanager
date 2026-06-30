using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Deadlines;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.ComputeDeadline;

public sealed class ComputeDeadlineCommandHandler(
    ICalendarEventRepository eventRepository,
    ICaseApi caseApi,
    ICalendarUnitOfWork unitOfWork) : ICommandHandler<ComputeDeadlineCommand, Result<ComputeDeadlineResponse>>
{
    public async Task<Result<ComputeDeadlineResponse>> Handle(ComputeDeadlineCommand request, CancellationToken cancellationToken = default)
    {
        DateOnly dueDate = LegalDeadlineCalculator.Compute(request.BaseDate, request.Type);

        if (!request.Schedule || request.OwnerUserId is not { } ownerUserId)
        {
            return Result.Success(new ComputeDeadlineResponse(dueDate, null));
        }

        if (request.CaseId is { } caseId && !await caseApi.CaseExistsAsync(caseId, cancellationToken))
        {
            return Result.Failure<ComputeDeadlineResponse>(CalendarErrors.CaseNotFound);
        }

        var start = dueDate.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc);
        CalendarEvent deadlineEvent = CalendarEvent.Schedule(
            ownerUserId,
            $"Échéance : {request.Type}",
            CalendarEventType.ProcedureDeadline,
            DateRange.Create(start, start.AddMinutes(30)),
            request.CaseId);

        eventRepository.Add(deadlineEvent);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ComputeDeadlineResponse(dueDate, deadlineEvent.Id.Value));
    }
}
