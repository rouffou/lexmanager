using FluentValidation;

namespace LexManager.Modules.Identity.Application.Features.RecordVerificationCheck;

public sealed class RecordVerificationCheckValidator : AbstractValidator<RecordVerificationCheckCommand>
{
    public RecordVerificationCheckValidator()
    {
        RuleFor(command => command.DueDiligenceId).NotEmpty();
        RuleFor(command => command.Kind).IsInEnum();
        RuleFor(command => command.Reference).NotEmpty().MaximumLength(256);
    }
}
