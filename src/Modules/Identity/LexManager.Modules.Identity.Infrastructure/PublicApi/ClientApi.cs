using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;

namespace LexManager.Modules.Identity.Infrastructure.PublicApi;

/// <summary>
/// Implementation of the Identity module's cross-module contract. Other modules resolve
/// <see cref="IClientApi"/> from DI — they never reference Identity's entities or DbContext.
/// </summary>
internal sealed class ClientApi(IClientReadRepository readRepository) : IClientApi
{
    public async Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        await readRepository.GetByIdAsync(clientId, cancellationToken) is not null;

    public async Task<ClientSummaryResponse?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        ClientResponse? client = await readRepository.GetByIdAsync(clientId, cancellationToken);

        return client is null
            ? null
            : new ClientSummaryResponse(client.Id, client.Type, client.DisplayName, client.Email, client.CreatedOnUtc);
    }
}
