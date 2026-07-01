using LexManager.Modules.CaseManagement.Domain.Procedures.Events;
using LexManager.SharedKernel.Domain;
using LexManager.SharedKernel.Exceptions;

namespace LexManager.Modules.CaseManagement.Domain.Procedures;

/// <summary>
/// The interactive procedure tree for a case (SRD V11 §36): an ordered set of milestone
/// <see cref="ProcedureStage"/>s generated from a <see cref="ProcedureType"/> blueprint. Exactly one
/// stage is <see cref="ProcedureStageStatus.Current"/> at a time (until the whole procedure is done),
/// letting the lawyer situate the case at a glance in the judicial calendar. One plan per case.
/// </summary>
public sealed class ProcedurePlan : AggregateRoot<ProcedurePlanId>
{
    private readonly List<ProcedureStage> _stages = [];

    private ProcedurePlan() { }

    private ProcedurePlan(ProcedurePlanId id, Guid caseId, ProcedureType type, DateTime referenceOnUtc) : base(id)
    {
        CaseId = caseId;
        Type = type;
        ReferenceOnUtc = referenceOnUtc;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid CaseId { get; private set; }
    public ProcedureType Type { get; private set; }

    /// <summary>The triggering date the calendar is anchored on (e.g. the act's date).</summary>
    public DateTime ReferenceOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public IReadOnlyList<ProcedureStage> Stages => _stages.AsReadOnly();

    public ProcedureStage? CurrentStage =>
        _stages.FirstOrDefault(stage => stage.Status == ProcedureStageStatus.Current);

    public int TotalStages => _stages.Count;

    public int ResolvedStages =>
        _stages.Count(stage => stage.Status is ProcedureStageStatus.Completed or ProcedureStageStatus.Skipped);

    public int ProgressPercent =>
        TotalStages == 0 ? 0 : (int)Math.Round(ResolvedStages * 100.0 / TotalStages, MidpointRounding.AwayFromZero);

    public bool IsComplete => TotalStages > 0 && ResolvedStages == TotalStages;

    /// <summary>Builds the tree for <paramref name="type"/> with the first stage set as current.</summary>
    public static ProcedurePlan Generate(Guid caseId, ProcedureType type, DateTime referenceOnUtc)
    {
        IReadOnlyList<ProcedureStageBlueprint> blueprint = ProcedureCatalog.For(type);
        if (blueprint.Count == 0)
        {
            throw new BusinessRuleValidationException(ProcedureErrors.EmptyProcedure);
        }

        var plan = new ProcedurePlan(ProcedurePlanId.New(), caseId, type, referenceOnUtc);

        int firstOrder = blueprint.Min(stage => stage.Order);
        foreach (ProcedureStageBlueprint stage in blueprint.OrderBy(stage => stage.Order))
        {
            ProcedureStageStatus status = stage.Order == firstOrder
                ? ProcedureStageStatus.Current
                : ProcedureStageStatus.Pending;
            plan._stages.Add(new ProcedureStage(stage.Order, stage.Name, stage.Phase, status));
        }

        plan.Raise(new ProcedurePlanGeneratedDomainEvent(plan.Id.Value, caseId, type.ToString()));
        return plan;
    }

    /// <summary>Marks the current stage as passed (franchie) and promotes the next pending stage.</summary>
    public void AdvanceCurrentStage()
    {
        ProcedureStage current = RequireCurrentStage();
        current.Complete();
        PromoteNextStage();
        Raise(new ProcedureStageAdvancedDomainEvent(Id.Value, current.Order, Skipped: false));
    }

    /// <summary>Marks the current stage as not applicable (ignorée) and promotes the next pending stage.</summary>
    public void SkipCurrentStage()
    {
        ProcedureStage current = RequireCurrentStage();
        current.Skip();
        PromoteNextStage();
        Raise(new ProcedureStageAdvancedDomainEvent(Id.Value, current.Order, Skipped: true));
    }

    /// <summary>Sets the planned date of a stage so it appears on the judicial calendar.</summary>
    public void ScheduleStage(int order, DateTime plannedOnUtc) => GetStage(order).Schedule(plannedOnUtc);

    private ProcedureStage RequireCurrentStage() =>
        CurrentStage ?? throw new BusinessRuleValidationException(ProcedureErrors.AlreadyComplete);

    private void PromoteNextStage()
    {
        ProcedureStage? next = _stages
            .Where(stage => stage.Status == ProcedureStageStatus.Pending)
            .OrderBy(stage => stage.Order)
            .FirstOrDefault();
        next?.MarkCurrent();
    }

    private ProcedureStage GetStage(int order) =>
        _stages.SingleOrDefault(stage => stage.Order == order)
            ?? throw new BusinessRuleValidationException(ProcedureErrors.UnknownStage);
}
