using LexManager.Modules.CaseManagement.Contracts;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GetCaseById;

public sealed record GetCaseByIdQuery(Guid CaseId) : IQuery<Result<CaseResponse>>;
