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
