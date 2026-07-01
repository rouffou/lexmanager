using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetProcedurePlan;

public sealed record GetProcedurePlanQuery(Guid CaseId) : IQuery<Result<ProcedurePlanResponse>>;
