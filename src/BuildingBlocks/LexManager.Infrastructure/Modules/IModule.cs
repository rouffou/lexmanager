using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Infrastructure.Modules;

/// <summary>
/// A self-contained functional module of the modular monolith. The Bootstrapper discovers
/// every <see cref="IModule"/> and wires it in isolation: registration first, endpoints last.
/// </summary>
public interface IModule
{
    /// <summary>Stable module name, used for logging, route prefixes and diagnostics.</summary>
    string Name { get; }

    /// <summary>
    /// Assemblies Mediarq must scan for this module's handlers, validators and event handlers
    /// (typically the module's Application assembly).
    /// </summary>
    IEnumerable<Assembly> Assemblies { get; }

    /// <summary>Registers the module's services, DbContext and integrations.</summary>
    IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration);

    /// <summary>Maps the module's HTTP endpoints (Minimal API).</summary>
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
