namespace LexManager.Modules.Identity.Application.Features.DecideDueDiligence;

public sealed record DecideDueDiligenceRequest(bool Approve, string? Reason);
