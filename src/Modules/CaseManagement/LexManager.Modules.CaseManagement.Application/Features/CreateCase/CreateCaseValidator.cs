using FluentValidation;

namespace LexManager.Modules.CaseManagement.Application.Features.CreateCase;

public sealed class CreateCaseValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseValidator()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(256);
        RuleFor(command => command.ClientId).NotEmpty();

        // If any jurisdiction field is supplied, court name and RG number are both required.
        When(command => command.CourtName is not null || command.GeneralRegisterNumber is not null, () =>
        {
            RuleFor(command => command.CourtName).NotEmpty();
            RuleFor(command => command.GeneralRegisterNumber).NotEmpty();
        });
    }
}
