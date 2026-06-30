using System.Reflection;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Modules;
using LexManager.Modules.Billing.Application;
using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Contracts;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Infrastructure.Numbering;
using LexManager.Modules.Billing.Infrastructure.Persistence;
using LexManager.Modules.Billing.Infrastructure.PublicApi;
using LexManager.Modules.Billing.Infrastructure.Reminders;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Modules.Billing.Infrastructure;

/// <summary>Composition root for the Billing &amp; Finance module (SRD Module 5).</summary>
public sealed class BillingModule : IModule
{
    public string Name => "Billing";

    public IEnumerable<Assembly> Assemblies => [typeof(ApplicationMarker).Assembly];

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("LexManager") ??
            configuration.GetConnectionString("Billing");

        services.AddDbContext<BillingDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", BillingDbContext.Schema)));

        services.AddScoped<IBillingUnitOfWork>(provider => provider.GetRequiredService<BillingDbContext>());
        services.AddScoped<IBillingDocumentRepository, BillingDocumentRepository>();
        services.AddScoped<IBillingReadRepository, BillingReadRepository>();
        services.AddScoped<IInvoiceNumberGenerator, SequentialInvoiceNumberGenerator>();
        services.AddScoped<IPaymentReminderSender, LoggingPaymentReminderSender>();
        services.AddScoped<IBillingApi, BillingApi>();

        services.AddEndpointsFrom(typeof(ApplicationMarker).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}
