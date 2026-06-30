using FluentValidation;

namespace LexManager.Modules.Identity.Application.Features.StartDueDiligence;

public sealed class StartDueDiligenceValidator : AbstractValidator<StartDueDiligenceCommand>
{
    public StartDueDiligenceValidator()
    {
        RuleFor(command => command.ClientId).NotEmpty();
        RuleFor(command => command.RiskLevel).IsInEnum();
    }
}
