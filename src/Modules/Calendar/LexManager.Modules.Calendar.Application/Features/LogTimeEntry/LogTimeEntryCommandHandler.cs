using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Domain.Common;
using LexManager.Modules.Calendar.Domain.TimeTracking;
using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.LogTimeEntry;

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
