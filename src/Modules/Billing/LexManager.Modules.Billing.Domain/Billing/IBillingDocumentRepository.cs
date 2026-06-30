using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Domain.Billing;

/// <summary>Write-side persistence port for the <see cref="BillingDocument"/> aggregate.</summary>
public interface IBillingDocumentRepository
{
    Task<BillingDocument?> GetByIdAsync(BillingDocumentId id, CancellationToken cancellationToken = default);

    /// <summary>Issued documents whose due date has passed — used by the overdue/reminder process.</summary>
    Task<IReadOnlyList<BillingDocument>> GetOverdueCandidatesAsync(DateTime asOfUtc, CancellationToken cancellationToken = default);

    void Add(BillingDocument document);
}
