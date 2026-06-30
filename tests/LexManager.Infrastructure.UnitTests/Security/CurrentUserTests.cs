using System.Security.Claims;
using LexManager.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace LexManager.Infrastructure.UnitTests.Security;

public class CurrentUserTests
{
    private static ICurrentUser ForClaims(params Claim[] claims)
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "test"))
        };
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return new CurrentUser(accessor);
    }

    [Fact]
    public void Should_Expose_UserId_Email_AndPermissions()
    {
        var id = Guid.NewGuid();
        ICurrentUser user = ForClaims(
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, "avocat@cabinet.fr"),
            new Claim(Permissions.ClaimType, Permissions.CasesWrite),
            new Claim(Permissions.ClaimType, Permissions.DocumentsReadConfidential));

        user.IsAuthenticated.Should().BeTrue();
        user.UserId.Should().Be(id);
        user.Email.Should().Be("avocat@cabinet.fr");
        user.HasPermission(Permissions.CasesWrite).Should().BeTrue();
        user.HasPermission(Permissions.DocumentsReadConfidential).Should().BeTrue();
        user.HasPermission(Permissions.AdminManage).Should().BeFalse();
    }

    [Fact]
    public void Anonymous_Context_Should_NotBeAuthenticated()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns((HttpContext?)null);

        ICurrentUser user = new CurrentUser(accessor);

        user.IsAuthenticated.Should().BeFalse();
        user.UserId.Should().BeNull();
        user.Permissions.Should().BeEmpty();
    }
}
