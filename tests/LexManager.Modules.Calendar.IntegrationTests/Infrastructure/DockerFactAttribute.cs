using System.Diagnostics;

namespace LexManager.Modules.Calendar.IntegrationTests.Infrastructure;

/// <summary>A <see cref="FactAttribute"/> that auto-skips when no Docker engine is reachable.</summary>
public sealed class DockerFactAttribute : FactAttribute
{
    public DockerFactAttribute()
    {
        if (!DockerEnvironment.IsAvailable.Value)
        {
            Skip = "Docker engine is not available; skipping Testcontainers integration test.";
        }
    }
}

internal static class DockerEnvironment
{
    public static readonly Lazy<bool> IsAvailable = new(Probe);

    private static bool Probe()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo("docker", "info")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            return process is not null && process.WaitForExit(8_000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
