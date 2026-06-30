using FluentValidation;

namespace LexManager.Modules.Documents.Application.Features.GenerateFromTemplate;

public sealed class GenerateFromTemplateValidator : AbstractValidator<GenerateFromTemplateCommand>
{
    public GenerateFromTemplateValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.TemplateKey).NotEmpty();
    }
}
