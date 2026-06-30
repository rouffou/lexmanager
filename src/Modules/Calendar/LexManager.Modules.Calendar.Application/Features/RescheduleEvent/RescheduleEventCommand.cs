using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.Events;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed record RescheduleEventCommand(Guid EventId, DateTime StartUtc, DateTime EndUtc) : ICommand<Result>;

public sealed class RescheduleEventValidator : AbstractValidator<RescheduleEventCommand>
{
    public RescheduleEventValidator() => RuleFor(command => command.EndUtc).GreaterThan(command => command.StartUtc);
}

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

public sealed record RescheduleEventRequest(DateTime StartUtc, DateTime EndUtc);
