using LexManager.Infrastructure.Modules;
using LexManager.Modules.Calendar.Infrastructure;
using LexManager.Modules.CaseManagement.Infrastructure;
using LexManager.Modules.Documents.Infrastructure;
using LexManager.Modules.Identity.Infrastructure;

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
        new IdentityModule(),
        new CaseManagementModule(),
        new DocumentsModule(),
        new CalendarModule(),
    ];
}
