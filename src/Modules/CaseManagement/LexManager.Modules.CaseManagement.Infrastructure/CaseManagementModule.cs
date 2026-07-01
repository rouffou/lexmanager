using System.Reflection;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Modules;
using LexManager.Modules.CaseManagement.Application;
using LexManager.Modules.CaseManagement.Application.Abstractions;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Domain.Procedures;
using LexManager.Modules.CaseManagement.Infrastructure.Persistence;
using LexManager.Modules.CaseManagement.Infrastructure.PublicApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Modules.CaseManagement.Infrastructure;

/// <summary>Composition root for the Case Management module (SRD Module 2).</summary>
public sealed class CaseManagementModule : IModule
{
    public string Name => "CaseManagement";

    public IEnumerable<Assembly> Assemblies => [typeof(ApplicationMarker).Assembly];

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("LexManager") ??
            configuration.GetConnectionString("CaseManagement");

        services.AddDbContext<CaseManagementDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", CaseManagementDbContext.Schema)));

        services.AddScoped<ICaseUnitOfWork>(provider => provider.GetRequiredService<CaseManagementDbContext>());
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<ICaseReadRepository, CaseReadRepository>();
        services.AddScoped<IProcedurePlanRepository, ProcedurePlanRepository>();
        services.AddScoped<IProcedureReadRepository, ProcedureReadRepository>();
        services.AddScoped<ICaseApi, CaseApi>();

        services.AddEndpointsFrom(typeof(ApplicationMarker).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}
