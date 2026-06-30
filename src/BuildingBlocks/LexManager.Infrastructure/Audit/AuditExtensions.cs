using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LexManager.Infrastructure.Audit;

public static class AuditExtensions
{
    /// <summary>Registers the audit sink and plugs <see cref="AuditBehavior{TRequest,TResponse}"/> into the Mediarq pipeline.</summary>
    public static IServiceCollection AddLexManagerAudit(this IServiceCollection services)
    {
        services.TryAddSingleton<IAuditSink, LoggingAuditSink>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
        return services;
    }
}
