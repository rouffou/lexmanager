using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Calendar.Infrastructure.Persistence;

internal sealed class CalendarReadRepository(CalendarDbContext context) : ICalendarReadRepository
{
    public async Task<CalendarEventResponse?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var id = new CalendarEventId(eventId);

        return await context.Events
            .AsNoTracking()
            .Where(calendarEvent => calendarEvent.Id == id)
            .Select(calendarEvent => new CalendarEventResponse(
                calendarEvent.Id.Value,
                calendarEvent.OwnerUserId,
                calendarEvent.Title,
                calendarEvent.Type.ToString(),
                calendarEvent.CaseId,
                calendarEvent.Period.StartUtc,
                calendarEvent.Period.EndUtc,
                calendarEvent.Location,
                calendarEvent.IsPrivate,
                calendarEvent.Provider.ToString()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedList<CalendarEventSummaryResponse>> GetAgendaAsync(
        Guid ownerUserId,
        DateTime fromUtc,
        DateTime toUtc,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Domain.Events.CalendarEvent> query = context.Events
            .AsNoTracking()
            .Where(calendarEvent =>
                calendarEvent.OwnerUserId == ownerUserId &&
                calendarEvent.Period.StartUtc < toUtc &&
                fromUtc < calendarEvent.Period.EndUtc);

        int totalCount = await query.CountAsync(cancellationToken);

        List<CalendarEventSummaryResponse> items = await query
            .OrderBy(calendarEvent => calendarEvent.Period.StartUtc)
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .Select(calendarEvent => new CalendarEventSummaryResponse(
                calendarEvent.Id.Value,
                calendarEvent.Title,
                calendarEvent.Type.ToString(),
                calendarEvent.Period.StartUtc,
                calendarEvent.Period.EndUtc,
                calendarEvent.IsPrivate))
            .ToListAsync(cancellationToken);

        return new PagedList<CalendarEventSummaryResponse>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<CaseTimeSummaryResponse> GetCaseTimeSummaryAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        var totals = await context.TimeEntries
            .AsNoTracking()
            .Where(timeEntry => timeEntry.CaseId == caseId)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Sum(timeEntry => timeEntry.DurationMinutes),
                Billable = group.Sum(timeEntry => timeEntry.IsBillable ? timeEntry.DurationMinutes : 0),
                Count = group.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new CaseTimeSummaryResponse(caseId, totals?.Total ?? 0, totals?.Billable ?? 0, totals?.Count ?? 0);
    }
}
