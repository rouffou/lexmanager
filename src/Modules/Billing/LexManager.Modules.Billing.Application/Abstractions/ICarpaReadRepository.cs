using LexManager.Modules.Billing.Contracts;

namespace LexManager.Modules.Billing.Application.Abstractions;

/// <summary>Read-side port (CQRS) for third-party (CARPA) accounts.</summary>
public interface ICarpaReadRepository
{
    Task<CarpaAccountResponse?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    Task<CarpaAccountResponse?> GetByCaseAsync(Guid caseId, CancellationToken cancellationToken = default);
}
