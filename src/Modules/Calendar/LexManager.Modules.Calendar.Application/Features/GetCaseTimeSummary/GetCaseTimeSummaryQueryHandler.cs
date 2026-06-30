using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetCaseTimeSummary;

public sealed class GetCaseTimeSummaryQueryHandler(ICalendarReadRepository readRepository)
    : IQueryHandler<GetCaseTimeSummaryQuery, Result<CaseTimeSummaryResponse>>
{
    public async Task<Result<CaseTimeSummaryResponse>> Handle(GetCaseTimeSummaryQuery request, CancellationToken cancellationToken = default)
    {
        CaseTimeSummaryResponse summary = await readRepository.GetCaseTimeSummaryAsync(request.CaseId, cancellationToken);
        return Result.Success(summary);
    }
}
