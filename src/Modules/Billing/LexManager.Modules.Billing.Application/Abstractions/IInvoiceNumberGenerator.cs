using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Application.Abstractions;

/// <summary>Generates the next legal document number for a given kind (e.g. FAC-2026-000123).</summary>
public interface IInvoiceNumberGenerator
{
    Task<string> NextAsync(BillingDocumentKind kind, CancellationToken cancellationToken = default);
}
