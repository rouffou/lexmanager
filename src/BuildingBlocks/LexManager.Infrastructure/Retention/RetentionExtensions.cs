using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Infrastructure.Retention;

public static class RetentionExtensions
{
    /// <summary>Registers the retention/maintenance background worker (config section <c>Retention</c>).</summary>
    public static IServiceCollection AddLexManagerRetention(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RetentionOptions>(configuration.GetSection("Retention"));
        services.AddHostedService<RetentionSweepService>();
        return services;
    }
}
