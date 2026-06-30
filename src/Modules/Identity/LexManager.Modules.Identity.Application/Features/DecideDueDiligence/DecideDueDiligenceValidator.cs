using FluentValidation;

namespace LexManager.Modules.Identity.Application.Features.DecideDueDiligence;

public sealed class DecideDueDiligenceValidator : AbstractValidator<DecideDueDiligenceCommand>
{
    public DecideDueDiligenceValidator()
    {
        RuleFor(command => command.DueDiligenceId).NotEmpty();
        RuleFor(command => command.Reason)
            .NotEmpty()
            .When(command => !command.Approve)
            .WithMessage("A reason is required when refusing the mandate.");
    }
}
