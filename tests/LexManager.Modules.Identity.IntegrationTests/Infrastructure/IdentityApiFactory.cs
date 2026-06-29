using LexManager.Modules.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace LexManager.Modules.Identity.IntegrationTests.Infrastructure;

/// <summary>
/// Spins up a real PostgreSQL container (Testcontainers, Normes §4.3) and points the Identity
/// DbContext at it, so feature tests exercise the full MediatR/Mediarq pipeline and EF Core
/// against a genuine database.
/// </summary>
public sealed class IdentityApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("lexmanager_test")
        .WithUsername("lex")
        .WithPassword("lex")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                service => service.ServiceType == typeof(DbContextOptions<IdentityDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(_database.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();

        using IServiceScope scope = Services.CreateScope();
        IdentityDbContext context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }
}
