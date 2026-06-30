using System.Reflection;
using LexManager.Infrastructure.Endpoints;
using LexManager.Infrastructure.Modules;
using LexManager.Modules.Documents.Application;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Contracts;
using LexManager.Modules.Documents.Domain.Documents;
using LexManager.Modules.Documents.Infrastructure.Persistence;
using LexManager.Modules.Documents.Infrastructure.PublicApi;
using LexManager.Modules.Documents.Infrastructure.Storage;
using LexManager.Modules.Documents.Infrastructure.Templates;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LexManager.Modules.Documents.Infrastructure;

/// <summary>Composition root for the Document Management module (SRD Module 3).</summary>
public sealed class DocumentsModule : IModule
{
    public string Name => "Documents";

    public IEnumerable<Assembly> Assemblies => [typeof(ApplicationMarker).Assembly];

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("LexManager") ??
            configuration.GetConnectionString("Documents");

        services.AddDbContext<DocumentsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__ef_migrations_history", DocumentsDbContext.Schema)));

        services.AddScoped<IDocumentUnitOfWork>(provider => provider.GetRequiredService<DocumentsDbContext>());
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentReadRepository, DocumentReadRepository>();
        services.AddScoped<IDocumentApi, DocumentApi>();

        string storageRoot = configuration["DocumentStorage:RootPath"]
            ?? Path.Combine(AppContext.BaseDirectory, "document-storage");
        services.AddSingleton<IDocumentStorage>(new FileSystemDocumentStorage(storageRoot));
        services.AddSingleton<ITemplateRenderer, SimpleTemplateRenderer>();

        services.AddEndpointsFrom(typeof(ApplicationMarker).Assembly);

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}
