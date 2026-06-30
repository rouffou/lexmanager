namespace LexManager.Modules.Billing.IntegrationTests.Infrastructure;

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
