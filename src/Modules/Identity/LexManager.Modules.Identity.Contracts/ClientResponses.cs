namespace LexManager.Modules.Identity.Contracts;

/// <summary>Full client read model returned by the API (never the domain entity, Normes §1.1).</summary>
public sealed record ClientResponse(
    Guid Id,
    string Type,
    string DisplayName,
    string Email,
    string? Phone,
    string? FirstName,
    string? LastName,
    string? NationalIdentityNumber,
    string? CompanyName,
    string? RegistrationNumber,
    string? LegalRepresentative,
    DateTime CreatedOnUtc);

/// <summary>Lightweight projection for paginated client lists.</summary>
public sealed record ClientSummaryResponse(
    Guid Id,
    string Type,
    string DisplayName,
    string Email,
    DateTime CreatedOnUtc);
