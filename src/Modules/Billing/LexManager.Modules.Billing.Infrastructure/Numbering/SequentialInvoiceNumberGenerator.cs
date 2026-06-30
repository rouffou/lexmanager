using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Billing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LexManager.Modules.Billing.Infrastructure.Numbering;

/// <summary>
/// Produces sequential, year-scoped legal document numbers (e.g. <c>FAC-2026-000001</c>).
/// Numbers must be continuous and unique per the accounting rules (SRD Module 5).
/// </summary>
internal sealed class SequentialInvoiceNumberGenerator(BillingDbContext context) : IInvoiceNumberGenerator
{
    public async Task<string> NextAsync(BillingDocumentKind kind, CancellationToken cancellationToken = default)
    {
        string prefix = kind switch
        {
            BillingDocumentKind.Invoice => "FAC",
            BillingDocumentKind.ProvisionCall => "APP",
            BillingDocumentKind.FeeNote => "NOTE",
            _ => "DOC"
        };

        string seed = $"{prefix}-{DateTime.UtcNow.Year}-";
        int issued = await context.Documents.CountAsync(
            document => document.Number != null && document.Number.StartsWith(seed), cancellationToken);

        return $"{seed}{issued + 1:D6}";
    }
}
