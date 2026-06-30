using Microsoft.Extensions.Logging;

namespace LexManager.Infrastructure.Audit;

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
