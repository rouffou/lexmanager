using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Calendar.Contracts;

public sealed record CalendarEventResponse(
    Guid Id,
    Guid OwnerUserId,
    string Title,
    string Type,
    Guid? CaseId,
    DateTime StartUtc,
    DateTime EndUtc,
    string? Location,
    bool IsPrivate,
    string Provider);

public sealed record CalendarEventSummaryResponse(
    Guid Id,
    string Title,
    string Type,
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsPrivate);

public sealed record TimeEntryResponse(
    Guid Id,
    Guid CaseId,
    Guid UserId,
    string Description,
    DateTime WorkedOnUtc,
    int DurationMinutes,
    bool IsBillable);

/// <summary>Aggregated time logged on a case — consumed by the Billing module for time-based fees.</summary>
public sealed record CaseTimeSummaryResponse(Guid CaseId, int TotalMinutes, int BillableMinutes, int EntryCount);

/// <summary>Cross-module contract exposing time-tracking totals.</summary>
public interface ITimeTrackingApi : IModuleApi
{
    Task<CaseTimeSummaryResponse> GetCaseTimeSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
