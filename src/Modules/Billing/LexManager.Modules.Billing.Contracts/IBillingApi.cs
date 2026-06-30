using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Billing.Contracts;

/// <summary>Billing module's public cross-module contract.</summary>
public interface IBillingApi : IModuleApi
{
    Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
