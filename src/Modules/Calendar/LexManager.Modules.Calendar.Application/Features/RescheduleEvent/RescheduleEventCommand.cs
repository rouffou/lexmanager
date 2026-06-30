using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.RescheduleEvent;

public sealed record RescheduleEventCommand(Guid EventId, DateTime StartUtc, DateTime EndUtc) : ICommand<Result>;
