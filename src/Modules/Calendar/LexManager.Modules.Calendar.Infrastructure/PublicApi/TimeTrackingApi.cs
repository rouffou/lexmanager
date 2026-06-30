using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;

namespace LexManager.Modules.Calendar.Infrastructure.PublicApi;

internal sealed class TimeTrackingApi(ICalendarReadRepository readRepository) : ITimeTrackingApi
{
    public Task<CaseTimeSummaryResponse> GetCaseTimeSummaryAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        readRepository.GetCaseTimeSummaryAsync(caseId, cancellationToken);
}
