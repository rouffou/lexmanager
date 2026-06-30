using LexManager.Modules.Calendar.Infrastructure.Persistence;
using LexManager.Modules.CaseManagement.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Testcontainers.PostgreSql;

namespace LexManager.Modules.Calendar.IntegrationTests.Infrastructure;

public sealed class CalendarApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
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
            ServiceDescriptor? dbContext = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<CalendarDbContext>));
            if (dbContext is not null)
            {
                services.Remove(dbContext);
            }

            services.AddDbContext<CalendarDbContext>(options => options.UseNpgsql(_database.GetConnectionString()));

            ServiceDescriptor? caseApi = services.SingleOrDefault(s => s.ServiceType == typeof(ICaseApi));
            if (caseApi is not null)
            {
                services.Remove(caseApi);
            }

            services.AddSingleton(CaseApi);
        });
    }

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        CaseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        using IServiceScope scope = Services.CreateScope();
        CalendarDbContext context = scope.ServiceProvider.GetRequiredService<CalendarDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }
}
