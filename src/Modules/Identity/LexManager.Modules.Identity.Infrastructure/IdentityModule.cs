using System.Reflection;
using LexManager.Application.Abstractions.Persistence;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Modules;
using LexManager.Modules.Identity.Application;
using LexManager.Modules.Identity.Application.Abstractions;
using LexManager.Modules.Identity.Contracts;
using LexManager.Modules.Identity.Domain.Clients;
using LexManager.Modules.Identity.Infrastructure.ConflictOfInterest;
using LexManager.Modules.Identity.Infrastructure.Persistence;
using LexManager.Modules.Identity.Infrastructure.PublicApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Modules.Identity.Infrastructure;

/// <summary>
/// Composition root for the Identity &amp; CRM module (SRD Module 1). Registered in the
/// Bootstrapper's <c>ModuleRegistry</c>; wires the module's persistence, services and endpoints
/// in complete isolation from other modules.
/// </summary>
public sealed class IdentityModule : IModule
{
    public string Name => "Identity";

    public IEnumerable<Assembly> Assemblies => [typeof(ApplicationMarker).Assembly];

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("LexManager") ??
            configuration.GetConnectionString("Identity");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", IdentityDbContext.Schema)));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IClientReadRepository, ClientReadRepository>();
        services.AddScoped<IConflictOfInterestChecker, ConflictOfInterestChecker>();
        services.AddScoped<IClientApi, ClientApi>();

        services.AddEndpointsFrom(typeof(ApplicationMarker).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}
