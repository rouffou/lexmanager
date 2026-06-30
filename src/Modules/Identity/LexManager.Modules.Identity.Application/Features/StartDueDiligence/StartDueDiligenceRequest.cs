using LexManager.Modules.Identity.Domain.Compliance;

namespace LexManager.Modules.Identity.Application.Features.StartDueDiligence;

public sealed record StartDueDiligenceRequest(RiskLevel RiskLevel = RiskLevel.Standard, bool IsPoliticallyExposed = false);
