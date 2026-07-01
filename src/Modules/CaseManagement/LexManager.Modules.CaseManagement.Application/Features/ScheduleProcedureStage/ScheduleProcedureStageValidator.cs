using FluentValidation;

namespace LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;

public sealed class ScheduleProcedureStageValidator : AbstractValidator<ScheduleProcedureStageCommand>
{
    public ScheduleProcedureStageValidator()
    {
        RuleFor(command => command.ProcedurePlanId).NotEmpty();
        RuleFor(command => command.StageOrder).GreaterThan(0);
        RuleFor(command => command.PlannedOnUtc).NotEmpty();
    }
}
