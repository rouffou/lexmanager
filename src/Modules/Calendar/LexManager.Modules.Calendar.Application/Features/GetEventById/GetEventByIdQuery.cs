using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetEventById;

public sealed record GetEventByIdQuery(Guid EventId) : IQuery<Result<CalendarEventResponse>>;
