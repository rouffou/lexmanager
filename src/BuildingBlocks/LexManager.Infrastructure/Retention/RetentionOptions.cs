namespace LexManager.Infrastructure.Retention;

public sealed class RetentionOptions
{
    /// <summary>Disabled by default so the worker only runs where explicitly configured.</summary>
    public bool Enabled { get; set; }

    /// <summary>How often the sweep runs. Defaults to daily.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(24);
}
