namespace LexManager.Modules.Billing.Contracts;

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
