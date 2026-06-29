using System.Diagnostics;

namespace LexManager.Modules.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// A <see cref="FactAttribute"/> that auto-skips when no Docker engine is reachable, so the
/// Testcontainers-backed integration suite runs in CI (where Docker is present) without breaking
/// local runs on machines without Docker.
/// </summary>
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

            if (process is null)
            {
                return false;
            }

            return process.WaitForExit(8_000) && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
