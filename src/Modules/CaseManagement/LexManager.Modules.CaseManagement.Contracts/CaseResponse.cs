namespace LexManager.Modules.CaseManagement.Contracts;

public sealed record CaseResponse(
    Guid Id,
    string Title,
    Guid ClientId,
    string Status,
    bool IsArchived,
    JurisdictionResponse? Jurisdiction,
    IReadOnlyList<AdversePartyResponse> AdverseParties,
    DateTime OpenedOnUtc,
    DateTime? ClosedOnUtc);
