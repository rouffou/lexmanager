using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Application.Features.AdvanceProcedureStage;
using LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;
using LexManager.Modules.CaseManagement.Application.Features.GetProcedurePlan;
using LexManager.Modules.CaseManagement.Application.Features.ScheduleProcedureStage;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using NSubstitute;

namespace LexManager.Modules.CaseManagement.UnitTests.Features;

public class ProcedureHandlersTests
{
    private readonly ICaseRepository _caseRepository = Substitute.For<ICaseRepository>();
    private readonly IProcedurePlanRepository _procedureRepository = Substitute.For<IProcedurePlanRepository>();
    private readonly IProcedureReadRepository _readRepository = Substitute.For<IProcedureReadRepository>();
    private readonly ICaseUnitOfWork _unitOfWork = Substitute.For<ICaseUnitOfWork>();

    private static ProcedurePlan CompletedPlan()
    {
        ProcedurePlan plan = ProcedurePlan.Generate(Guid.NewGuid(), ProcedureType.SummaryProceedings, DateTime.UtcNow);
        for (int i = 0; i < plan.TotalStages; i++)
        {
            plan.AdvanceCurrentStage();
        }

        return plan;
    }

    // ── Generate ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Generate_Should_Fail_WhenCaseMissing()
    {
        _caseRepository.GetByIdAsync(Arg.Any<CaseId>(), Arg.Any<CancellationToken>()).Returns((Case?)null);
        var handler = new GenerateProcedurePlanCommandHandler(_caseRepository, _procedureRepository, _unitOfWork);

        var result = await handler.Handle(
            new GenerateProcedurePlanCommand(Guid.NewGuid(), ProcedureType.DebtRecovery, DateTime.UtcNow),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.CaseNotFound);
        _procedureRepository.DidNotReceive().Add(Arg.Any<ProcedurePlan>());
    }

    [Fact]
    public async Task Generate_Should_Fail_WhenPlanAlreadyExists()
    {
        _caseRepository.GetByIdAsync(Arg.Any<CaseId>(), Arg.Any<CancellationToken>()).Returns(Case.Open("Dossier", Guid.NewGuid()));
        _procedureRepository.ExistsForCaseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new GenerateProcedurePlanCommandHandler(_caseRepository, _procedureRepository, _unitOfWork);

        var result = await handler.Handle(
            new GenerateProcedurePlanCommand(Guid.NewGuid(), ProcedureType.DebtRecovery, DateTime.UtcNow),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.AlreadyExistsForCase);
    }

    [Fact]
    public async Task Generate_Should_Persist_WhenValid()
    {
        _caseRepository.GetByIdAsync(Arg.Any<CaseId>(), Arg.Any<CancellationToken>()).Returns(Case.Open("Dossier", Guid.NewGuid()));
        _procedureRepository.ExistsForCaseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new GenerateProcedurePlanCommandHandler(_caseRepository, _procedureRepository, _unitOfWork);

        var result = await handler.Handle(
            new GenerateProcedurePlanCommand(Guid.NewGuid(), ProcedureType.DebtRecovery, DateTime.UtcNow),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _procedureRepository.Received(1).Add(Arg.Is<ProcedurePlan>(plan => plan.Type == ProcedureType.DebtRecovery));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ── Advance / Skip ──────────────────────────────────────────────────────

    [Fact]
    public async Task Advance_Should_Fail_WhenPlanMissing()
    {
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns((ProcedurePlan?)null);
        var handler = new AdvanceProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(new AdvanceProcedureStageCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.NotFound);
    }

    [Fact]
    public async Task Advance_Should_Fail_WhenPlanComplete()
    {
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns(CompletedPlan());
        var handler = new AdvanceProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(new AdvanceProcedureStageCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.AlreadyComplete);
    }

    [Fact]
    public async Task Advance_Should_MoveToNextStage_AndSave()
    {
        ProcedurePlan plan = ProcedurePlan.Generate(Guid.NewGuid(), ProcedureType.DebtRecovery, DateTime.UtcNow);
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns(plan);
        var handler = new AdvanceProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(new AdvanceProcedureStageCommand(plan.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        plan.CurrentStage!.Order.Should().Be(2);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Skip_Should_MarkCurrentSkipped()
    {
        ProcedurePlan plan = ProcedurePlan.Generate(Guid.NewGuid(), ProcedureType.DebtRecovery, DateTime.UtcNow);
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns(plan);
        var handler = new AdvanceProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(new AdvanceProcedureStageCommand(plan.Id.Value, Skip: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        plan.Stages[0].Status.Should().Be(ProcedureStageStatus.Skipped);
    }

    // ── Schedule ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Schedule_Should_Fail_WhenPlanMissing()
    {
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns((ProcedurePlan?)null);
        var handler = new ScheduleProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(
            new ScheduleProcedureStageCommand(Guid.NewGuid(), 1, DateTime.UtcNow), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.NotFound);
    }

    [Fact]
    public async Task Schedule_Should_Fail_ForUnknownStage()
    {
        ProcedurePlan plan = ProcedurePlan.Generate(Guid.NewGuid(), ProcedureType.SummaryProceedings, DateTime.UtcNow);
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns(plan);
        var handler = new ScheduleProcedureStageCommandHandler(_procedureRepository, _unitOfWork);

        var result = await handler.Handle(
            new ScheduleProcedureStageCommand(plan.Id.Value, 99, DateTime.UtcNow), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.UnknownStage);
    }

    [Fact]
    public async Task Schedule_Should_SetPlannedDate_AndSave()
    {
        ProcedurePlan plan = ProcedurePlan.Generate(Guid.NewGuid(), ProcedureType.SummaryProceedings, DateTime.UtcNow);
        _procedureRepository.GetByIdAsync(Arg.Any<ProcedurePlanId>(), Arg.Any<CancellationToken>()).Returns(plan);
        var handler = new ScheduleProcedureStageCommandHandler(_procedureRepository, _unitOfWork);
        var planned = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = await handler.Handle(
            new ScheduleProcedureStageCommand(plan.Id.Value, 2, planned), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        plan.Stages.Single(stage => stage.Order == 2).PlannedOnUtc.Should().Be(planned);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ── Get ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Get_Should_ReturnNotFound_WhenAbsent()
    {
        _readRepository.GetByCaseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ProcedurePlanResponse?)null);
        var handler = new GetProcedurePlanQueryHandler(_readRepository);

        var result = await handler.Handle(new GetProcedurePlanQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProcedureErrors.NotFound);
    }

    [Fact]
    public async Task Get_Should_ReturnPlan_WhenPresent()
    {
        var caseId = Guid.NewGuid();
        var response = new ProcedurePlanResponse(
            Guid.NewGuid(), caseId, "DebtRecovery", DateTime.UtcNow, 0, 1, false, [], DateTime.UtcNow);
        _readRepository.GetByCaseAsync(caseId, Arg.Any<CancellationToken>()).Returns(response);
        var handler = new GetProcedurePlanQueryHandler(_readRepository);

        var result = await handler.Handle(new GetProcedurePlanQuery(caseId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.CaseId.Should().Be(caseId);
    }
}
