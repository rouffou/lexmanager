using System.Reflection;
using LexManager.Modules.Identity.Application;
using LexManager.Modules.Identity.Domain.Clients;
using LexManager.Modules.Identity.Infrastructure;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

/// <summary>
/// Clean Architecture + modular-monolith rules for the Identity module (Normes §4.1):
/// the Domain depends on nothing technical, and layers point inward only.
/// </summary>
public class IdentityModuleRulesTests
{
    private static readonly Assembly Domain = typeof(Client).Assembly;
    private static readonly Assembly Application = typeof(ApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(IdentityModule).Assembly;

    [Fact]
    public void Domain_Should_Not_DependOn_Application_Or_Infrastructure()
    {
        TestResult result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Identity.Application",
                "LexManager.Modules.Identity.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_Not_DependOn_EfCore_Or_AspNetCore()
    {
        TestResult result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_DependOn_Infrastructure()
    {
        TestResult result = Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOn("LexManager.Modules.Identity.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_DependOn_EfCore()
    {
        TestResult result = Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("reads/writes go through ports; EF Core stays in Infrastructure");
    }
}
