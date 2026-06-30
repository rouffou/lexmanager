using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;

public sealed class RecordCarpaDisbursementValidator : AbstractValidator<RecordCarpaDisbursementCommand>
{
    public RecordCarpaDisbursementValidator()
    {
        RuleFor(command => command.AccountId).NotEmpty();
        RuleFor(command => command.Amount).GreaterThan(0m);
        RuleFor(command => command.Description).NotEmpty();
    }
}
