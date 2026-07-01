using LexManager.Modules.CaseManagement.Domain.Procedures;
using LexManager.Modules.CaseManagement.Domain.Procedures.Events;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.UnitTests.Domain;

public class ProcedurePlanTests
{
    private static ProcedurePlan NewPlan(ProcedureType type = ProcedureType.SummaryProceedings) =>
        ProcedurePlan.Generate(Guid.NewGuid(), type, new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void Generate_Should_BuildStages_WithFirstCurrent_AndRaiseEvent()
    {
        ProcedurePlan plan = NewPlan();

        plan.TotalStages.Should().Be(4);
        plan.Stages[0].Status.Should().Be(ProcedureStageStatus.Current);
        plan.Stages.Skip(1).Should().OnlyContain(stage => stage.Status == ProcedureStageStatus.Pending);
        plan.CurrentStage!.Order.Should().Be(1);
        plan.ProgressPercent.Should().Be(0);
        plan.IsComplete.Should().BeFalse();
        plan.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProcedurePlanGeneratedDomainEvent>();
    }

    [Fact]
    public void Generate_Should_Throw_ForUnknownProcedureType()
    {
        Action act = () => ProcedurePlan.Generate(Guid.NewGuid(), (ProcedureType)999, DateTime.UtcNow);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ProcedureErrors.EmptyProcedure);
    }

    [Fact]
    public void AdvanceCurrentStage_Should_CompleteCurrent_AndPromoteNext()
    {
        ProcedurePlan plan = NewPlan();
        plan.ClearDomainEvents();

        plan.AdvanceCurrentStage();

        plan.Stages[0].Status.Should().Be(ProcedureStageStatus.Completed);
        plan.Stages[0].CompletedOnUtc.Should().NotBeNull();
        plan.Stages[1].Status.Should().Be(ProcedureStageStatus.Current);
        plan.CurrentStage!.Order.Should().Be(2);
        plan.ProgressPercent.Should().Be(25);
        plan.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProcedureStageAdvancedDomainEvent>()
            .Which.Skipped.Should().BeFalse();
    }

    [Fact]
    public void SkipCurrentStage_Should_MarkSkipped_AndPromoteNext()
    {
        ProcedurePlan plan = NewPlan();
        plan.ClearDomainEvents();

        plan.SkipCurrentStage();

        plan.Stages[0].Status.Should().Be(ProcedureStageStatus.Skipped);
        plan.Stages[0].CompletedOnUtc.Should().BeNull();
        plan.CurrentStage!.Order.Should().Be(2);
        plan.ProgressPercent.Should().Be(25);
        plan.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProcedureStageAdvancedDomainEvent>()
            .Which.Skipped.Should().BeTrue();
    }

    [Fact]
    public void Advancing_AllStages_Should_CompleteThePlan()
    {
        ProcedurePlan plan = NewPlan();

        for (int i = 0; i < plan.TotalStages; i++)
        {
            plan.AdvanceCurrentStage();
        }

        plan.IsComplete.Should().BeTrue();
        plan.CurrentStage.Should().BeNull();
        plan.ProgressPercent.Should().Be(100);
    }

    [Fact]
    public void AdvanceCurrentStage_Should_Throw_WhenComplete()
    {
        ProcedurePlan plan = NewPlan();
        for (int i = 0; i < plan.TotalStages; i++)
        {
            plan.AdvanceCurrentStage();
        }

        Action act = () => plan.AdvanceCurrentStage();

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ProcedureErrors.AlreadyComplete);
    }

    [Fact]
    public void ScheduleStage_Should_SetPlannedDate()
    {
        ProcedurePlan plan = NewPlan();
        var planned = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        plan.ScheduleStage(2, planned);

        plan.Stages.Single(stage => stage.Order == 2).PlannedOnUtc.Should().Be(planned);
    }

    [Fact]
    public void ScheduleStage_Should_Throw_ForUnknownOrder()
    {
        ProcedurePlan plan = NewPlan();

        Action act = () => plan.ScheduleStage(99, DateTime.UtcNow);

        act.Should().Throw<BusinessRuleValidationException>()
            .Which.Error.Should().Be(ProcedureErrors.UnknownStage);
    }
}
