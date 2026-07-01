using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

internal sealed class ProcedureReadRepository(CaseManagementDbContext context) : IProcedureReadRepository
{
    public async Task<ProcedurePlanResponse?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default)
    {
        ProcedurePlan? plan = await context.ProcedurePlans
            .AsNoTracking()
            .FirstOrDefaultAsync(plan => plan.CaseId == caseId, cancellationToken);

        return plan is null ? null : Map(plan);
    }

    private static ProcedurePlanResponse Map(ProcedurePlan plan) => new(
        plan.Id.Value,
        plan.CaseId,
        plan.Type.ToString(),
        plan.ReferenceOnUtc,
        plan.ProgressPercent,
        plan.CurrentStage?.Order,
        plan.IsComplete,
        plan.Stages
            .OrderBy(stage => stage.Order)
            .Select(stage => new ProcedureStageResponse(
                stage.Order,
                stage.Name,
                stage.Phase,
                stage.Status.ToString(),
                stage.PlannedOnUtc,
                stage.CompletedOnUtc))
            .ToList(),
        plan.CreatedOnUtc);
}
