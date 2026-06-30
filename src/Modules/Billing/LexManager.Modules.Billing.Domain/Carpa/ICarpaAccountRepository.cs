namespace LexManager.Modules.Billing.Domain.Carpa;

/// <summary>Write-side persistence port for the <see cref="CarpaAccount"/> aggregate.</summary>
public interface ICarpaAccountRepository
{
    Task<CarpaAccount?> GetByIdAsync(CarpaAccountId id, CancellationToken cancellationToken = default);

    Task<CarpaAccount?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForCaseAsync(Guid caseId, CancellationToken cancellationToken = default);

    void Add(CarpaAccount account);
}
