using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

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
