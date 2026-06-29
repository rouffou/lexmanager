using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Identity.Contracts;

/// <summary>
/// Identity module's public, cross-module API. Other modules (e.g. Case Management, which links
/// cases to clients) depend on THIS contract, not on Identity's internals or database (SRD §3.2).
/// </summary>
public interface IClientApi : IModuleApi
{
    Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<ClientSummaryResponse?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default);
}
