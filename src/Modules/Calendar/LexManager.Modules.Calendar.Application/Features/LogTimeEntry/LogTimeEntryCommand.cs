using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.LogTimeEntry;

public sealed record LogTimeEntryCommand(
    Guid CaseId,
    Guid UserId,
    string Description,
    int DurationMinutes,
    bool IsBillable = true,
    DateTime? WorkedOnUtc = null) : ICommand<Result<Guid>>;
