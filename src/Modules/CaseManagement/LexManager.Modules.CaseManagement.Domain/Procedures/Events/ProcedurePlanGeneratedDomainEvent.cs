using LexManager.SharedKernel.Domain;

namespace LexManager.Modules.CaseManagement.Domain.Procedures.Events;

public sealed record ProcedurePlanGeneratedDomainEvent(Guid ProcedurePlanId, Guid CaseId, string ProcedureType)
    : IDomainEvent;
