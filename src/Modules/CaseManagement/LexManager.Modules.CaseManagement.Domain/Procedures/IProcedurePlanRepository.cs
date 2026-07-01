namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>Write-side persistence port for the <see cref="ProcedurePlan"/> aggregate.</summary>
public interface IProcedurePlanRepository
{
    Task<ProcedurePlan?> GetByIdAsync(ProcedurePlanId id, CancellationToken cancellationToken = default);

    Task<ProcedurePlan?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForCaseAsync(Guid caseId, CancellationToken cancellationToken = default);

    void Add(ProcedurePlan plan);
}
