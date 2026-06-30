using LexManager.Application.Abstractions.Messaging;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexManager.Infrastructure.Retention;

public sealed class RetentionOptions
{
    /// <summary>Disabled by default so the worker only runs where explicitly configured.</summary>
    public bool Enabled { get; set; }

    /// <summary>How often the sweep runs. Defaults to daily.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(24);
}

/// <summary>
/// Background worker (HostedService) that periodically triggers the legal-retention purge and the
/// overdue-payment review by publishing notifications (SRD §5.3 / §6). Modules react to those
/// notifications, so the worker never reaches into their storage.
/// </summary>
public sealed class RetentionSweepService(
    IServiceScopeFactory scopeFactory,
    IOptions<RetentionOptions> options,
    ILogger<RetentionSweepService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RetentionOptions settings = options.Value;
        if (!settings.Enabled)
        {
            logger.LogInformation("Retention worker is disabled (set Retention:Enabled=true to activate).");
            return;
        }

        using var timer = new PeriodicTimer(settings.Interval);
        logger.LogInformation("Retention worker started; sweeping every {Interval}.", settings.Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                DateTime now = DateTime.UtcNow;
                await publisher.Publish(new PaymentReminderSweepRequested(now), stoppingToken);
                await publisher.Publish(new RetentionSweepRequested(now), stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Retention sweep iteration failed.");
            }
        }
    }
}
