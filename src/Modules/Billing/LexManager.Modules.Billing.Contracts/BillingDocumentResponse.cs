namespace LexManager.Modules.Billing.Contracts;

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
