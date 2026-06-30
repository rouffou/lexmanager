using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Billing.Domain.Interest;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Billing.Application.Features.SimulateLegalInterest;

public sealed class SimulateLegalInterestQueryHandler
    : IQueryHandler<SimulateLegalInterestQuery, Result<LegalInterestResponse>>
{
    public Task<Result<LegalInterestResponse>> Handle(SimulateLegalInterestQuery request, CancellationToken cancellationToken = default)
    {
        LegalInterestResult result = BelgianLegalInterestCalculator.Compute(
            Money.Of(request.Principal, request.Currency),
            request.FromDate,
            request.ToDate,
            request.AnnualRatePercent,
            request.Capitalize);

        var response = new LegalInterestResponse(
            result.Principal.Amount,
            result.Principal.Currency,
            result.FromDate,
            result.ToDate,
            result.Days,
            result.AnnualRatePercent,
            result.Capitalized,
            result.Interest.Amount,
            result.Total.Amount);

        return Task.FromResult(Result.Success(response));
    }
}
