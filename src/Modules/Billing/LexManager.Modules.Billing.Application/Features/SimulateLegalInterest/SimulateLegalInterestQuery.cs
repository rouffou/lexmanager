using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.SimulateLegalInterest;

/// <summary>Simulates Belgian legal default interest on a condemnation (SRD V11 §5).</summary>
public sealed record SimulateLegalInterestQuery(
    decimal Principal,
    DateOnly FromDate,
    DateOnly ToDate,
    decimal AnnualRatePercent,
    bool Capitalize = false,
    string Currency = Money.DefaultCurrency) : IQuery<Result<LegalInterestResponse>>;
