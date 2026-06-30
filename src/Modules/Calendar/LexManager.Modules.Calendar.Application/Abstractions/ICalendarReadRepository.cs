using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Calendar.Contracts;

namespace LexManager.Modules.Calendar.Application.Abstractions;

/// <summary>Read-side port (CQRS) returning flat DTOs.</summary>
public interface ICalendarReadRepository
{
    Task<CalendarEventResponse?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<PagedList<CalendarEventSummaryResponse>> GetAgendaAsync(
        Guid ownerUserId,
        DateTime fromUtc,
        DateTime toUtc,
        PaginationParameters parameters,
        CancellationToken cancellationToken = default);

    Task<CaseTimeSummaryResponse> GetCaseTimeSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
