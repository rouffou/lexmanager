using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.ScheduleEvent;

public sealed record ScheduleEventCommand(
    Guid OwnerUserId,
    string Title,
    CalendarEventType Type,
    DateTime StartUtc,
    DateTime EndUtc,
    Guid? CaseId = null,
    string? Location = null,
    bool IsPrivate = false,
    bool AllowOverlap = false) : ICommand<Result<Guid>>;

public sealed class ScheduleEventValidator : AbstractValidator<ScheduleEventCommand>
{
    public ScheduleEventValidator()
    {
        RuleFor(command => command.OwnerUserId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(256);
        RuleFor(command => command.EndUtc).GreaterThan(command => command.StartUtc);
    }
}

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
