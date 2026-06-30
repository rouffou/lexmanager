using System.Diagnostics;
using LexManager.Infrastructure.Security;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace LexManager.Infrastructure.Audit;

/// <summary>
/// Mediarq pipeline behavior that writes an audit-trail entry for every request — recording the
/// action (the command/query name), the authenticated user, the outcome and the duration. This
/// gives complete traceability of who consulted or modified what and when (SRD §5.1).
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse>(ICurrentUser currentUser, IAuditSink auditSink)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        string action = typeof(TRequest).Name;
        Guid? userId = currentUser.UserId;
        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            TResponse response = await handle();

            await auditSink.WriteAsync(
                new AuditEntry(DateTime.UtcNow, action, userId, "Success", ElapsedMs(startTimestamp)),
                cancellationToken);

            return response;
        }
        catch
        {
            await auditSink.WriteAsync(
                new AuditEntry(DateTime.UtcNow, action, userId, "Failure", ElapsedMs(startTimestamp)),
                cancellationToken);
            throw;
        }
    }

    private static long ElapsedMs(long startTimestamp) =>
        (long)Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
}
