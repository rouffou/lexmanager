using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.SimulateLegalInterest;

public sealed class SimulateLegalInterestValidator : AbstractValidator<SimulateLegalInterestQuery>
{
    public SimulateLegalInterestValidator()
    {
        RuleFor(query => query.Principal).GreaterThan(0m);
        RuleFor(query => query.AnnualRatePercent).GreaterThan(0m);
        RuleFor(query => query.ToDate).GreaterThan(query => query.FromDate);
    }
}
