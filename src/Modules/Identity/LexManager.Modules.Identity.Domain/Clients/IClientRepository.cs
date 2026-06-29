namespace LexManager.Modules.Identity.Domain.Clients;

/// <summary>Write-side persistence port for the <see cref="Client"/> aggregate.</summary>
public interface IClientRepository
{
    Task<Client?> GetByIdAsync(ClientId id, CancellationToken cancellationToken = default);

    void Add(Client client);
}
