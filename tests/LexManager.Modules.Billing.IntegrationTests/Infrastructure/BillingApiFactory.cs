using LexManager.Modules.Billing.Infrastructure.Persistence;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace LexManager.Modules.Billing.IntegrationTests.Infrastructure;

public sealed class BillingApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("lexmanager_test")
        .WithUsername("lex")
        .WithPassword("lex")
        .Build();

    public ICaseApi CaseApi { get; } = Substitute.For<ICaseApi>();
    public IClientApi ClientApi { get; } = Substitute.For<IClientApi>();
    public ITimeTrackingApi TimeTrackingApi { get; } = Substitute.For<ITimeTrackingApi>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Replace<DbContextOptions<BillingDbContext>>(services);
            services.AddDbContext<BillingDbContext>(options => options.UseNpgsql(_database.GetConnectionString()));

            Replace<ICaseApi>(services);
            services.AddSingleton(CaseApi);
            Replace<IClientApi>(services);
            services.AddSingleton(ClientApi);
            Replace<ITimeTrackingApi>(services);
            services.AddSingleton(TimeTrackingApi);
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        CaseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        ClientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        using IServiceScope scope = Services.CreateScope();
        BillingDbContext context = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
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
}
