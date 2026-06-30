using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.AddBillingLine;

public sealed class AddBillingLineValidator : AbstractValidator<AddBillingLineCommand>
{
    public AddBillingLineValidator()
    {
        RuleFor(command => command.Description).NotEmpty();
        RuleFor(command => command.Quantity).GreaterThan(0m);
        RuleFor(command => command.UnitPrice).GreaterThanOrEqualTo(0m);
    }
}
