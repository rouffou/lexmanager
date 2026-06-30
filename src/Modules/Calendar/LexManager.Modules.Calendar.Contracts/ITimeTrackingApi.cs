using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Calendar.Contracts;

/// <summary>Cross-module contract exposing time-tracking totals.</summary>
public interface ITimeTrackingApi : IModuleApi
{
    Task<CaseTimeSummaryResponse> GetCaseTimeSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
