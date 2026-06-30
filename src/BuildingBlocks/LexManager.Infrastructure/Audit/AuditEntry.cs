namespace LexManager.Infrastructure.Audit;

/// <summary>A single audit record: who did what, when, and with what outcome (SRD §5.1).</summary>
public sealed record AuditEntry(
    DateTime TimestampUtc,
    string Action,
    Guid? UserId,
    string Outcome,
    long ElapsedMilliseconds);
