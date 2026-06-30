using System.Collections.Concurrent;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Documents.Application.Abstractions;
using LexManager.Modules.Documents.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace LexManager.Modules.Documents.IntegrationTests.Infrastructure;

public sealed class DocumentsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("lexmanager_test")
        .WithUsername("lex")
        .WithPassword("lex")
        .Build();

    public ICaseApi CaseApi { get; } = Substitute.For<ICaseApi>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Replace<DbContextOptions<DocumentsDbContext>>(services);
            services.AddDbContext<DocumentsDbContext>(options => options.UseNpgsql(_database.GetConnectionString()));

            ReplaceService<ICaseApi>(services, _ => CaseApi);
            ReplaceService<IDocumentStorage>(services, _ => new InMemoryDocumentStorage());
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        CaseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        using IServiceScope scope = Services.CreateScope();
        DocumentsDbContext context = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }

    private static void Replace<TService>(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(TService));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    private static void ReplaceService<TService>(IServiceCollection services, Func<IServiceProvider, TService> factory)
        where TService : class
    {
        Replace<TService>(services);
        services.AddSingleton(factory);
    }
}

/// <summary>In-memory <see cref="IDocumentStorage"/> so integration tests don't touch the filesystem.</summary>
internal sealed class InMemoryDocumentStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<string, byte[]> _files = new();

    public Task<StoredFile> SaveAsync(byte[] content, CancellationToken cancellationToken = default)
    {
        string key = Guid.NewGuid().ToString("N");
        _files[key] = content;
        string checksum = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(content));
        return Task.FromResult(new StoredFile(key, content.LongLength, checksum));
    }

    public Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(_files.TryGetValue(storageKey, out byte[]? content) ? content : []);

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        _files.TryRemove(storageKey, out _);
        return Task.CompletedTask;
    }
}
