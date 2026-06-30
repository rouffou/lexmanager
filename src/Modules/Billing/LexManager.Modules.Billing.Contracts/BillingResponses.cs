using LexManager.Application.Abstractions.Messaging;

namespace LexManager.Modules.Billing.Contracts;

public sealed record InvoiceLineResponse(string Description, decimal Quantity, decimal UnitPrice, decimal LineTotal);

public sealed record BillingDocumentResponse(
    Guid Id,
    Guid CaseId,
    Guid ClientId,
    string Kind,
    string Mode,
    string Status,
    string VatRegime,
    string? LegalMention,
    string? Number,
    string Currency,
    decimal Subtotal,
    decimal TaxRatePercent,
    decimal TaxAmount,
    decimal Total,
    IReadOnlyList<InvoiceLineResponse> Lines,
    DateTime? IssuedOnUtc,
    DateTime? DueDateUtc,
    DateTime? PaidOnUtc,
    DateTime CreatedOnUtc);

public sealed record BillingDocumentSummaryResponse(
    Guid Id,
    string Kind,
    string Status,
    string? Number,
    decimal Total,
    string Currency,
    DateTime? DueDateUtc,
    DateTime CreatedOnUtc);

public sealed record CaseBillingSummaryResponse(
    Guid CaseId,
    decimal TotalInvoiced,
    decimal TotalPaid,
    decimal TotalOutstanding,
    string Currency,
    int DocumentCount);

public sealed record LegalInterestResponse(
    decimal Principal,
    string Currency,
    DateOnly FromDate,
    DateOnly ToDate,
    int Days,
    decimal AnnualRatePercent,
    bool Capitalized,
    decimal Interest,
    decimal Total);

/// <summary>Billing module's public cross-module contract.</summary>
public interface IBillingApi : IModuleApi
{
    Task<CaseBillingSummaryResponse> GetCaseBillingSummaryAsync(Guid caseId, CancellationToken cancellationToken = default);
}
