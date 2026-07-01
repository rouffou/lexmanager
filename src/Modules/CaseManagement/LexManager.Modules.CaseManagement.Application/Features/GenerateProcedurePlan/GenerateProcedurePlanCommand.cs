using LexManager.Modules.CaseManagement.Domain.Procedures;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;

public sealed record GenerateProcedurePlanCommand(Guid CaseId, ProcedureType ProcedureType, DateTime ReferenceOnUtc)
    : ICommand<Result<Guid>>;
