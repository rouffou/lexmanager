using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Deadlines;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.ComputeDeadline;

/// <summary>Computes a legal deadline and, optionally, places it on the agenda (SRD Module 4).</summary>
public sealed record ComputeDeadlineCommand(
    DateOnly BaseDate,
    LegalDeadlineType Type,
    Guid? OwnerUserId = null,
    Guid? CaseId = null,
    bool Schedule = false) : ICommand<Result<ComputeDeadlineResponse>>;

public sealed record ComputeDeadlineResponse(DateOnly DueDate, Guid? ScheduledEventId);

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

public sealed class ComputeDeadlineEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/calendar/deadlines/compute", async (
                ComputeDeadlineCommand command,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                Result<ComputeDeadlineResponse> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult();
            })
            .WithName("ComputeDeadline")
            .WithTags("Calendar")
            .Produces<ComputeDeadlineResponse>();
    }
}
