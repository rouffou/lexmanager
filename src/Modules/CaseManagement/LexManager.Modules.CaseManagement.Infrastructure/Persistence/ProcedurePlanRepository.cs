using LexManager.Modules.CaseManagement.Domain.Procedures;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.CaseManagement.Infrastructure.Persistence;

internal sealed class ProcedurePlanRepository(CaseManagementDbContext context) : IProcedurePlanRepository
{
    public Task<ProcedurePlan?> GetByIdAsync(ProcedurePlanId id, CancellationToken cancellationToken = default) =>
        context.ProcedurePlans.FirstOrDefaultAsync(plan => plan.Id == id, cancellationToken);

    public Task<ProcedurePlan?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        context.ProcedurePlans.FirstOrDefaultAsync(plan => plan.CaseId == caseId, cancellationToken);

    public Task<bool> ExistsForCaseAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        context.ProcedurePlans.AnyAsync(plan => plan.CaseId == caseId, cancellationToken);

    public void Add(ProcedurePlan plan) => context.ProcedurePlans.Add(plan);
}
