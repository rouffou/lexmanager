using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.CaseManagement.Domain.Procedures.Events;

public sealed record ProcedureStageAdvancedDomainEvent(Guid ProcedurePlanId, int StageOrder, bool Skipped)
    : IDomainEvent;
