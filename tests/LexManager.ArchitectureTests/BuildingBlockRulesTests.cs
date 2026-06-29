using LexManager.SharedKernel.Domain;
using NetArchTest.Rules;

namespace LexManager.ArchitectureTests;

/// <summary>
/// Guards the dependency rules of the shared kernel. Per-module Clean Architecture rules
/// (Domain ⇏ Infrastructure, no cross-module Infrastructure references) are added alongside
/// each module's tests (Normes §4.1).
/// </summary>
public class BuildingBlockRulesTests
{
    private static readonly System.Reflection.Assembly SharedKernel = typeof(Entity<>).Assembly;

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Infrastructure()
    {
        TestResult result = Types.InAssembly(SharedKernel)
            .ShouldNot()
            .HaveDependencyOn("LexManager.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "the shared kernel models the domain and must not reference any infrastructure");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_AspNetCore_Or_EfCore()
    {
        TestResult result = Types.InAssembly(SharedKernel)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "domain primitives must stay free of web and persistence concerns");
    }
}
