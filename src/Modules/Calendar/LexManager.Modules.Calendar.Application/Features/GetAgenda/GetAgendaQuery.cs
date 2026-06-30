using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetAgenda;

public sealed record GetAgendaQuery(Guid OwnerUserId, DateTime FromUtc, DateTime ToUtc, int Page = 1, int PageSize = 50)
    : IQuery<Result<PagedList<CalendarEventSummaryResponse>>>;
