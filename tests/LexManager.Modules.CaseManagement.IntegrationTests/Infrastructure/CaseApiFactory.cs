using LexManager.Modules.CaseManagement.Infrastructure.Persistence;
using LexManager.Modules.Identity.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace LexManager.Modules.CaseManagement.IntegrationTests.Infrastructure;

/// <summary>
/// Boots the API against a real PostgreSQL container for Case Management, and substitutes the
/// Identity module's <see cref="IClientApi"/> so the test exercises Case Management in isolation
/// (cross-module contract is verified, not Identity's internals).
/// </summary>
public sealed class CaseApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("lexmanager_test")
        .WithUsername("lex")
        .WithPassword("lex")
        .Build();

    public IClientApi ClientApi { get; } = Substitute.For<IClientApi>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? dbContext = services.SingleOrDefault(
                service => service.ServiceType == typeof(DbContextOptions<CaseManagementDbContext>));
            if (dbContext is not null)
            {
                services.Remove(dbContext);
            }

            services.AddDbContext<CaseManagementDbContext>(options =>
                options.UseNpgsql(_database.GetConnectionString()));

            ServiceDescriptor? clientApi = services.SingleOrDefault(service => service.ServiceType == typeof(IClientApi));
            if (clientApi is not null)
            {
                services.Remove(clientApi);
            }

            services.AddSingleton(ClientApi);
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        ClientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        using IServiceScope scope = Services.CreateScope();
        CaseManagementDbContext context = scope.ServiceProvider.GetRequiredService<CaseManagementDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }
}
