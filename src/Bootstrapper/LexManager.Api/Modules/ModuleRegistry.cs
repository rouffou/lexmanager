using LexManager.Infrastructure.Modules;

namespace LexManager.Api.Modules;

/// <summary>
/// Single, explicit place where the monolith's functional modules are composed.
/// Adding a module = adding one line here; the Bootstrapper does the rest (DI, Mediarq
/// assembly scanning, endpoint mapping) generically.
/// </summary>
public static class ModuleRegistry
{
    public static IReadOnlyList<IModule> Modules { get; } =
    [
        // Modules are registered here as they are implemented, e.g.:
        // new IdentityModule(),
        // new CaseManagementModule(),
    ];
}
