namespace LexManager.Modules.Billing.Contracts;

public sealed record CaseBillingSummaryResponse(
    Guid CaseId,
    decimal TotalInvoiced,
    decimal TotalPaid,
    decimal TotalOutstanding,
    string Currency,
    int DocumentCount);
