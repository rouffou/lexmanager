using LexManager.Modules.Identity.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Identity.Infrastructure.Persistence;

internal sealed class ClientRepository(IdentityDbContext context) : IClientRepository
{
    public Task<Client?> GetByIdAsync(ClientId id, CancellationToken cancellationToken = default) =>
        context.Clients.FirstOrDefaultAsync(client => client.Id == id, cancellationToken);

    public void Add(Client client) => context.Clients.Add(client);
}
