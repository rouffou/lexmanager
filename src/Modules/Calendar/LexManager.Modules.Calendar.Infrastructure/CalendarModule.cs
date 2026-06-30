using System.Reflection;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Modules;
using LexManager.Modules.Calendar.Application;
using LexManager.Modules.Calendar.Application.Abstractions;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.Calendar.Domain;
using LexManager.Modules.Calendar.Infrastructure.Persistence;
using LexManager.Modules.Calendar.Infrastructure.PublicApi;
using LexManager.Modules.Calendar.Infrastructure.Sync;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Modules.Calendar.Infrastructure;

/// <summary>Composition root for the Calendar &amp; Time-Tracking module (SRD Module 4).</summary>
public sealed class CalendarModule : IModule
{
    public string Name => "Calendar";

    public IEnumerable<Assembly> Assemblies => [typeof(ApplicationMarker).Assembly];

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("LexManager") ??
            configuration.GetConnectionString("Calendar");

        services.AddDbContext<CalendarDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", CalendarDbContext.Schema)));

        services.AddScoped<ICalendarUnitOfWork>(provider => provider.GetRequiredService<CalendarDbContext>());
        services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
        services.AddScoped<ITimeEntryRepository, TimeEntryRepository>();
        services.AddScoped<ICalendarReadRepository, CalendarReadRepository>();
        services.AddScoped<ITimeTrackingApi, TimeTrackingApi>();
        services.AddSingleton<ICalendarSyncProvider, NoOpCalendarSyncProvider>();

        services.AddEndpointsFrom(typeof(ApplicationMarker).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}
