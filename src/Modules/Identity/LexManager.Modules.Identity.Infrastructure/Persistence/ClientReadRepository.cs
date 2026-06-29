using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Clients;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Read path (CQRS). Uses <c>AsNoTracking()</c> and DB-side paging; value objects/computed
/// members are flattened to DTOs after materialization (Normes §1.1).
/// </summary>
internal sealed class ClientReadRepository(IdentityDbContext context) : IClientReadRepository
{
    public async Task<ClientResponse?> GetByIdAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var id = new ClientId(clientId);

        Client? client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return client is null ? null : MapFull(client);
    }

    public async Task<PagedList<ClientSummaryResponse>> GetPagedAsync(
        PaginationParameters parameters,
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Client> query = context.Clients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            query = query.Where(c =>
                (c.NationalIdentityNumber != null && c.NationalIdentityNumber.Contains(term)) ||
                (c.Name != null && (c.Name.FirstName.Contains(term) || c.Name.LastName.Contains(term))) ||
                (c.Organization != null && c.Organization.CompanyName.Contains(term)));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Client> clients = await query
            .OrderByDescending(c => c.CreatedOnUtc)
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        IReadOnlyList<ClientSummaryResponse> items = clients.Select(MapSummary).ToList();

        return new PagedList<ClientSummaryResponse>(items, parameters.Page, parameters.PageSize, totalCount);
    }

    private static ClientResponse MapFull(Client client) => new(
        client.Id.Value,
        client.Type.ToString(),
        client.DisplayName,
        client.Email.Value,
        client.Phone,
        client.Name?.FirstName,
        client.Name?.LastName,
        client.NationalIdentityNumber,
        client.Organization?.CompanyName,
        client.Organization?.RegistrationNumber,
        client.Organization?.LegalRepresentative,
        client.CreatedOnUtc);

    private static ClientSummaryResponse MapSummary(Client client) => new(
        client.Id.Value,
        client.Type.ToString(),
        client.DisplayName,
        client.Email.Value,
        client.CreatedOnUtc);
}
