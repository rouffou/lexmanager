using LexManager.Modules.Identity.Domain.Compliance;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.StartDueDiligence;

/// <summary>Opens an anti-money-laundering due-diligence file for a client (LCB-FT, SRD V11 §30).</summary>
public sealed record StartDueDiligenceCommand(
    Guid ClientId,
    RiskLevel RiskLevel = RiskLevel.Standard,
    bool IsPoliticallyExposed = false) : ICommand<Result<Guid>>;
