namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>Write-side persistence port for the <see cref="ClientDueDiligence"/> aggregate.</summary>
public interface IClientDueDiligenceRepository
{
    Task<ClientDueDiligence?> GetByIdAsync(DueDiligenceId id, CancellationToken cancellationToken = default);

    Task<ClientDueDiligence?> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    void Add(ClientDueDiligence file);
}
