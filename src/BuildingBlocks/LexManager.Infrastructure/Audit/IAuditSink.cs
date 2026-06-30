namespace LexManager.Infrastructure.Audit;

/// <summary>Sink for the audit trail. The default logs; a production sink persists to an append-only store.</summary>
public interface IAuditSink
{
    Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
