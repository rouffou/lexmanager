using LexManager.Modules.Billing.Domain.Common;

namespace LexManager.Modules.Billing.Domain.Interest;

/// <summary>Outcome of a Belgian legal-interest simulation.</summary>
public sealed record LegalInterestResult(
    Money Principal,
    DateOnly FromDate,
    DateOnly ToDate,
    int Days,
    decimal AnnualRatePercent,
    bool Capitalized,
    Money Interest,
    Money Total);
