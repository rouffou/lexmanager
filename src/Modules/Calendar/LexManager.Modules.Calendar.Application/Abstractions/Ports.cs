using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain.Common;
using Mediarq.UnitOfWork;

namespace LexManager.Modules.Calendar.Application.Abstractions;

/// <summary>Module-scoped unit of work (Mediarq's <see cref="IUnitOfWork"/>) for Calendar.</summary>
public interface ICalendarUnitOfWork : IUnitOfWork;

/// <summary>Event data pushed to an external calendar.</summary>
public sealed record CalendarSyncRequest(
    string Title,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location,
    bool MaskDetails);

/// <summary>Identifier returned by the external calendar after a successful push.</summary>
public sealed record CalendarSyncLink(CalendarProvider Provider, string ExternalId, string? ETag);

/// <summary>
/// Bidirectional synchronisation with an external calendar (Microsoft Graph / Google Calendar).
/// The default implementation is a no-op; real providers push events, honour the
/// <see cref="CalendarSyncRequest.MaskDetails"/> flag, and feed webhook callbacks back in (SRD Module 4).
/// </summary>
public interface ICalendarSyncProvider
{
    Task<CalendarSyncLink?> PushAsync(CalendarSyncRequest request, CancellationToken cancellationToken = default);

    Task HandleWebhookAsync(string provider, string payload, CancellationToken cancellationToken = default);
}

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
