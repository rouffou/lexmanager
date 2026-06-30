using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.DecideDueDiligence;

/// <summary>Accepts or refuses the mandate based on the due-diligence outcome (LCB-FT, SRD V11 §30).</summary>
public sealed record DecideDueDiligenceCommand(Guid DueDiligenceId, bool Approve, string? Reason) : ICommand<Result>;
