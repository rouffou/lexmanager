using LexManager.Modules.Calendar.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Calendar.Application.Features.GetCaseTimeSummary;

public sealed record GetCaseTimeSummaryQuery(Guid CaseId) : IQuery<Result<CaseTimeSummaryResponse>>;
