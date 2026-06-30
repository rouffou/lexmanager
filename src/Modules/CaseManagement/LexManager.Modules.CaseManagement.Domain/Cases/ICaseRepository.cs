namespace LexManager.Modules.CaseManagement.Domain.Cases;

/// <summary>Write-side persistence port for the <see cref="Case"/> aggregate.</summary>
public interface ICaseRepository
{
    Task<Case?> GetByIdAsync(CaseId id, CancellationToken cancellationToken = default);

    void Add(Case @case);
}
