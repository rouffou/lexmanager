namespace LexManager.Modules.CaseManagement.Contracts;

public sealed record AdversePartyResponse(string Name, string? Counsel);

public sealed record JurisdictionResponse(string CourtName, string GeneralRegisterNumber, string? Judge);

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

public sealed record CaseSummaryResponse(
    Guid Id,
    string Title,
    Guid ClientId,
    string Status,
    DateTime OpenedOnUtc);
