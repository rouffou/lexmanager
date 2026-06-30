using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetAgenda;

public sealed record GetAgendaQuery(Guid OwnerUserId, DateTime FromUtc, DateTime ToUtc, int Page = 1, int PageSize = 50)
    : IQuery<Result<PagedList<CalendarEventSummaryResponse>>>;

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

public sealed class GetAgendaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/calendar/events", async (
                Guid ownerUserId,
                DateTime from,
                DateTime to,
                ISender sender,
                CancellationToken cancellationToken,
                int page = 1,
                int pageSize = 50) =>
            {
                Result<PagedList<CalendarEventSummaryResponse>> result =
                    await sender.Send(new GetAgendaQuery(ownerUserId, from, to, page, pageSize), cancellationToken);
                return result.ToApiResult();
            })
            .WithName("GetAgenda")
            .WithTags("Calendar")
            .Produces<PagedList<CalendarEventSummaryResponse>>();
    }
}
