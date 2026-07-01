using LexManager.Modules.CaseManagement.Contracts;

namespace LexManager.Modules.CaseManagement.Application.Abstractions;

/// <summary>Read-side port (CQRS) for the procedure tree, returning flat DTOs.</summary>
public interface IProcedureReadRepository
{
    Task<ProcedurePlanResponse?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
