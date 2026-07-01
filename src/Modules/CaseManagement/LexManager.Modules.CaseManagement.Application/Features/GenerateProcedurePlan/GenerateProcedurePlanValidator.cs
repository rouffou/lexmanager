using FluentValidation;

namespace LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;

public sealed class GenerateProcedurePlanValidator : AbstractValidator<GenerateProcedurePlanCommand>
{
    public GenerateProcedurePlanValidator()
    {
        RuleFor(command => command.CaseId).NotEmpty();
        RuleFor(command => command.ProcedureType).IsInEnum();
        RuleFor(command => command.ReferenceOnUtc).NotEmpty();
    }
}
