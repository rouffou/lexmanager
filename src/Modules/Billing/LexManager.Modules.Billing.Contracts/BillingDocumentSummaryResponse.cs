namespace LexManager.Modules.Billing.Contracts;

public sealed record BillingDocumentSummaryResponse(
    Guid Id,
    string Kind,
    string Status,
    string? Number,
    decimal Total,
    string Currency,
    DateTime? DueDateUtc,
    DateTime CreatedOnUtc);
