using LexManager.Infrastructure.Audit;

namespace LexManager.Infrastructure.UnitTests.Audit;

public sealed class RecordingAuditSink : IAuditSink
{
    public List<AuditEntry> Entries { get; } = [];

    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}
