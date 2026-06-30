using System.Reflection;
using LexManager.Modules.Billing.Application;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Infrastructure;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

public class BillingModuleRulesTests
{
    private static readonly Assembly Domain = typeof(BillingDocument).Assembly;
    private static readonly Assembly Application = typeof(ApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(BillingModule).Assembly;

    [Fact]
    public void Domain_Should_Not_DependOn_Application_Or_Infrastructure()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Billing.Application",
                "LexManager.Modules.Billing.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_Not_DependOn_EfCore_Or_AspNetCore()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_DependOn_Infrastructure_Or_EfCore()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Billing.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Billing_Should_Only_Reach_OtherModules_Through_Contracts()
    {
        // Billing reads from three other modules — only their Contracts assemblies are allowed.
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Domain",
                "LexManager.Modules.CaseManagement.Application",
                "LexManager.Modules.CaseManagement.Infrastructure",
                "LexManager.Modules.Identity.Domain",
                "LexManager.Modules.Identity.Application",
                "LexManager.Modules.Identity.Infrastructure",
                "LexManager.Modules.Calendar.Domain",
                "LexManager.Modules.Calendar.Application",
                "LexManager.Modules.Calendar.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Billing_Infrastructure_Should_Not_DependOn_Other_Modules_Infrastructure()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Infrastructure",
                "LexManager.Modules.Identity.Infrastructure",
                "LexManager.Modules.Calendar.Infrastructure",
                "LexManager.Modules.Documents.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }
}
