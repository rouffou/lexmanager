using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;

public sealed class RecordCarpaDepositValidator : AbstractValidator<RecordCarpaDepositCommand>
{
    public RecordCarpaDepositValidator()
    {
        RuleFor(command => command.AccountId).NotEmpty();
        RuleFor(command => command.Amount).GreaterThan(0m);
        RuleFor(command => command.Description).NotEmpty();
    }
}
