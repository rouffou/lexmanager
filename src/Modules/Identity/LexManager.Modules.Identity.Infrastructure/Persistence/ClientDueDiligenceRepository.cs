using LexManager.Modules.Identity.Domain.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Identity.Infrastructure.Persistence;

internal sealed class ClientDueDiligenceRepository(IdentityDbContext context) : IClientDueDiligenceRepository
{
    public Task<ClientDueDiligence?> GetByIdAsync(DueDiligenceId id, CancellationToken cancellationToken = default) =>
        context.DueDiligenceFiles.FirstOrDefaultAsync(file => file.Id == id, cancellationToken);

    public Task<ClientDueDiligence?> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        context.DueDiligenceFiles.FirstOrDefaultAsync(file => file.ClientId == clientId, cancellationToken);

    public Task<bool> ExistsForClientAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        context.DueDiligenceFiles.AnyAsync(file => file.ClientId == clientId, cancellationToken);

    public void Add(ClientDueDiligence file) => context.DueDiligenceFiles.Add(file);
}
