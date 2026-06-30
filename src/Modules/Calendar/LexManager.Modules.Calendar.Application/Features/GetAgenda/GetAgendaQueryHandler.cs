using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetAgenda;

public sealed class GetAgendaQueryHandler(ICalendarReadRepository readRepository)
    : IQueryHandler<GetAgendaQuery, Result<PagedList<CalendarEventSummaryResponse>>>
{
    public async Task<Result<PagedList<CalendarEventSummaryResponse>>> Handle(
        GetAgendaQuery request,
        CancellationToken cancellationToken = default)
    {
        var parameters = new PaginationParameters(request.Page, request.PageSize);
        PagedList<CalendarEventSummaryResponse> page = await readRepository.GetAgendaAsync(
            request.OwnerUserId, request.FromUtc, request.ToUtc, parameters, cancellationToken);

        return Result.Success(page);
    }
}
