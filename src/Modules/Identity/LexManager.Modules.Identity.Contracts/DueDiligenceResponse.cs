namespace LexManager.Modules.Identity.Contracts;

public sealed record DueDiligenceResponse(
    Guid Id,
    Guid ClientId,
    string Status,
    string RiskLevel,
    bool IsPoliticallyExposed,
    int ComplianceScore,
    bool CanApprove,
    IReadOnlyList<string> RequiredChecks,
    IReadOnlyList<VerificationCheckResponse> Checks,
    DateTime OpenedOnUtc,
    DateTime? DecidedOnUtc,
    string? DecisionReason);
