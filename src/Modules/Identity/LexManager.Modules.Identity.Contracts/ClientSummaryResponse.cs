namespace LexManager.Modules.Identity.Contracts;

/// <summary>Lightweight projection for paginated client lists.</summary>
public sealed record ClientSummaryResponse(
    Guid Id,
    string Type,
    string DisplayName,
    string Email,
    DateTime CreatedOnUtc);
