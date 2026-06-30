namespace LexManager.Modules.Identity.Contracts;

public sealed record VerificationCheckResponse(
    string Kind,
    string Reference,
    bool Cleared,
    string? Notes,
    DateTime RecordedOnUtc);
