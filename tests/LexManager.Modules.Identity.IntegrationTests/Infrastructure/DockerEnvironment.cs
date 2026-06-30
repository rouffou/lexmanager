using System.Diagnostics;

namespace LexManager.Modules.Identity.IntegrationTests.Infrastructure;

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
