using LexManager.Application.Abstractions.Pagination;
using LexManager.Modules.Identity.Contracts;

namespace LexManager.Modules.Identity.Application.Abstractions;

/// <summary>
/// Read-side port. Returns flat, optimized DTOs directly (CQRS read path, Normes §1.1) —
/// implemented with EF Core <c>AsNoTracking()</c> projections in the Infrastructure layer.
/// </summary>
public interface IClientReadRepository
{
    Task<ClientResponse?> GetByIdAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<PagedList<ClientSummaryResponse>> GetPagedAsync(
        PaginationParameters parameters,
        string? search,
        CancellationToken cancellationToken = default);
}
