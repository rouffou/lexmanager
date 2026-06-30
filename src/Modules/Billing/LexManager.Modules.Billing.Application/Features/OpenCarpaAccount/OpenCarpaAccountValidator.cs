using FluentValidation;

namespace LexManager.Modules.Billing.Application.Features.OpenCarpaAccount;

public sealed class OpenCarpaAccountValidator : AbstractValidator<OpenCarpaAccountCommand>
{
    public OpenCarpaAccountValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ClientId).NotEmpty();
    }
}
