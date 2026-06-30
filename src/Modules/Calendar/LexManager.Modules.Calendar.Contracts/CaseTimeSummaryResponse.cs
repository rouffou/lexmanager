namespace LexManager.Modules.Calendar.Contracts;

/// <summary>Aggregated time logged on a case — consumed by the Billing module for time-based fees.</summary>
public sealed record CaseTimeSummaryResponse(Guid CaseId, int TotalMinutes, int BillableMinutes, int EntryCount);
