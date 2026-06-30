using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Billing.Infrastructure.Persistence;

internal sealed class BillingDocumentRepository(BillingDbContext context) : IBillingDocumentRepository
{
    public Task<BillingDocument?> GetByIdAsync(BillingDocumentId id, CancellationToken cancellationToken = default) =>
        context.Documents.FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

    public async Task<IReadOnlyList<BillingDocument>> GetOverdueCandidatesAsync(DateTime asOfUtc, CancellationToken cancellationToken = default) =>
        await context.Documents
            .Where(document => document.Status == BillingStatus.Issued && document.DueDateUtc != null && document.DueDateUtc < asOfUtc)
            .ToListAsync(cancellationToken);

    public void Add(BillingDocument document) => context.Documents.Add(document);
}
