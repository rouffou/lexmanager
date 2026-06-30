using System.Reflection;
using LexManager.Modules.Documents.Application;
using LexManager.Modules.Documents.Domain.Documents;
using LexManager.Modules.Documents.Infrastructure;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

public class DocumentsModuleRulesTests
{
    private static readonly Assembly Domain = typeof(Document).Assembly;
    private static readonly Assembly Application = typeof(ApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(DocumentsModule).Assembly;

    [Fact]
    public void Domain_Should_Not_DependOn_Application_Or_Infrastructure()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Documents.Application",
                "LexManager.Modules.Documents.Infrastructure")
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
                "LexManager.Modules.Documents.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Documents_Should_Only_Reach_OtherModules_Through_Contracts()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Domain",
                "LexManager.Modules.CaseManagement.Application",
                "LexManager.Modules.CaseManagement.Infrastructure",
                "LexManager.Modules.Identity.Domain",
                "LexManager.Modules.Identity.Application",
                "LexManager.Modules.Identity.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Documents_Infrastructure_Should_Not_DependOn_Other_Modules_Infrastructure()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Infrastructure",
                "LexManager.Modules.Identity.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }
}
