using Microsoft.Extensions.Logging;

namespace LexManager.Infrastructure.Audit;

/// <summary>A single audit record: who did what, when, and with what outcome (SRD §5.1).</summary>
public sealed record AuditEntry(
    DateTime TimestampUtc,
    string Action,
    Guid? UserId,
    string Outcome,
    long ElapsedMilliseconds);

/// <summary>Sink for the audit trail. The default logs; a production sink persists to an append-only store.</summary>
public interface IAuditSink
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}

public sealed class LoggingAuditSink(ILogger<LoggingAuditSink> logger) : IAuditSink
{
    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "AUDIT {Action} by {UserId} -> {Outcome} in {Elapsed}ms at {Timestamp:o}",
            entry.Action, entry.UserId?.ToString() ?? "anonymous", entry.Outcome, entry.ElapsedMilliseconds, entry.TimestampUtc);
        return Task.CompletedTask;
    }
}
