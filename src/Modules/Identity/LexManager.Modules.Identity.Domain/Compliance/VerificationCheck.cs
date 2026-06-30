namespace LexManager.Modules.Identity.Domain.Compliance;

/// <summary>
/// One verification performed during client due diligence (e.g. an identity document or a PEP
/// screening). Owned by <see cref="ClientDueDiligence"/>; there is at most one check per kind.
/// </summary>
public sealed class VerificationCheck
{
    private VerificationCheck() { }

    internal VerificationCheck(VerificationKind kind, string reference, bool cleared, string? notes)
    {
        Kind = kind;
        Reference = reference;
        Cleared = cleared;
        Notes = notes;
        RecordedOnUtc = DateTime.UtcNow;
    }

    public VerificationKind Kind { get; private set; }
    public string Reference { get; private set; } = null!;
    public bool Cleared { get; private set; }
    public string? Notes { get; private set; }
    public DateTime RecordedOnUtc { get; private set; }

    internal void Update(string reference, bool cleared, string? notes)
    {
        Reference = reference;
        Cleared = cleared;
        Notes = notes;
        RecordedOnUtc = DateTime.UtcNow;
    }
}
