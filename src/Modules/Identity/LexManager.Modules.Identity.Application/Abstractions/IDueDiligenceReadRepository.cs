using LexManager.Modules.Identity.Contracts;

namespace LexManager.Modules.Identity.Application.Abstractions;

/// <summary>Read-side port (CQRS) for client due-diligence (KYC / LCB-FT) files.</summary>
public interface IDueDiligenceReadRepository
{
    Task<DueDiligenceResponse?> GetByIdAsync(Guid dueDiligenceId, CancellationToken cancellationToken = default);

    Task<DueDiligenceResponse?> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<bool> IsClientApprovedAsync(Guid clientId, CancellationToken cancellationToken = default);
}
