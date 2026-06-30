using System.Reflection;
using LexManager.Modules.CaseManagement.Application;
using LexManager.Modules.CaseManagement.Domain.Cases;
using LexManager.Modules.CaseManagement.Infrastructure;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

public class CaseManagementModuleRulesTests
{
    private static readonly Assembly Domain = typeof(Case).Assembly;
    private static readonly Assembly Application = typeof(ApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(CaseManagementModule).Assembly;

    [Fact]
    public void Domain_Should_Not_DependOn_Application_Or_Infrastructure()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Application",
                "LexManager.Modules.CaseManagement.Infrastructure")
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
                "LexManager.Modules.CaseManagement.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CaseManagement_Should_Only_Reach_Identity_Through_Contracts()
    {
        // Cross-module communication is allowed only via the public Contracts assembly (SRD §3.2).
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Identity.Domain",
                "LexManager.Modules.Identity.Application",
                "LexManager.Modules.Identity.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CaseManagement_Infrastructure_Should_Not_DependOn_Identity_Infrastructure()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("LexManager.Modules.Identity.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }
}
