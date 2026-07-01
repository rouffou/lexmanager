using LexManager.Modules.CaseManagement.Domain.Procedures;

namespace LexManager.Modules.CaseManagement.Application.Features.GenerateProcedurePlan;

/// <summary>Request body for generating a case's procedure tree (the case id comes from the route).</summary>
public sealed record GenerateProcedurePlanRequest(ProcedureType ProcedureType, DateTime ReferenceOnUtc);
