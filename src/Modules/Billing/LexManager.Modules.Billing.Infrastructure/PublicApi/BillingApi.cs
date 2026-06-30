using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;

namespace LexManager.Modules.Billing.Infrastructure.PublicApi;

internal sealed class BillingApi(IBillingReadRepository readRepository) : IBillingApi
{
    public Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default) =>
        readRepository.GetCaseBillingSummaryAsync(caseId, cancellationToken);
}
