using FluentValidation;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.LogTimeEntry;

public sealed record LogTimeEntryCommand(
    Guid CaseId,
    Guid UserId,
    string Description,
    int DurationMinutes,
    bool IsBillable = true,
    DateTime? WorkedOnUtc = null) : ICommand<Result<Guid>>;

public sealed class LogTimeEntryValidator : AbstractValidator<LogTimeEntryCommand>
{
    public LogTimeEntryValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DurationMinutes).GreaterThan(0);
    }
}

public sealed class LogTimeEntryCommandHandler(
    ITimeEntryRepository timeEntryRepository,
    ICaseApi caseApi,
    ICalendarUnitOfWork unitOfWork) : ICommandHandler<LogTimeEntryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(LogTimeEntryCommand request, CancellationToken cancellationToken = default)
    {
        if (!await caseApi.CaseExistsAsync(request.CaseId, cancellationToken))
        {
            return Result.Failure<Guid>(CalendarErrors.CaseNotFound);
        }

        TimeEntry entry = TimeEntry.Log(
            request.CaseId, request.UserId, request.Description, request.DurationMinutes, request.IsBillable, request.WorkedOnUtc);

        timeEntryRepository.Add(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.Id.Value);
    }
}

public sealed class LogTimeEntryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/time-entries", async (LogTimeEntryCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> result = await sender.Send(command, cancellationToken);
                return result.ToApiResult(id => Results.Created($"/api/time-entries/{id}", new { id }));
            })
            .WithName("LogTimeEntry")
            .WithTags("Time tracking")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }
}
