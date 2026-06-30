using LexManager.Application.Abstractions.Pagination;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Results;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LexManager.Modules.Calendar.Application.Features.GetAgenda;

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
