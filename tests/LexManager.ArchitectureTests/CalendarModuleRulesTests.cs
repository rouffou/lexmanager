using System.Reflection;
using LexManager.Modules.Calendar.Application;
using LexManager.Modules.Calendar.Domain.Events;
using LexManager.Modules.Calendar.Infrastructure;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

public class CalendarModuleRulesTests
{
    private static readonly Assembly Domain = typeof(CalendarEvent).Assembly;
    private static readonly Assembly Application = typeof(ApplicationMarker).Assembly;
    private static readonly Assembly Infrastructure = typeof(CalendarModule).Assembly;

    [Fact]
    public void Domain_Should_Not_DependOn_Application_Or_Infrastructure()
    {
        Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.Calendar.Application",
                "LexManager.Modules.Calendar.Infrastructure")
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
                "LexManager.Modules.Calendar.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Calendar_Should_Only_Reach_OtherModules_Through_Contracts()
    {
        Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Domain",
                "LexManager.Modules.CaseManagement.Application",
                "LexManager.Modules.CaseManagement.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Calendar_Infrastructure_Should_Not_DependOn_Other_Modules_Infrastructure()
    {
        Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(
                "LexManager.Modules.CaseManagement.Infrastructure",
                "LexManager.Modules.Documents.Infrastructure",
                "LexManager.Modules.Identity.Infrastructure")
            .GetResult().IsSuccessful.Should().BeTrue();
    }
}
