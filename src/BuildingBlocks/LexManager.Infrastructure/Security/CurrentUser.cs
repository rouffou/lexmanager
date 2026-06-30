using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LexManager.Infrastructure.Security;

/// <summary>Ambient information about the authenticated caller, derived from the JWT claims.</summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    string? Email { get; }
    IReadOnlySet<string> Permissions { get; }
    bool HasPermission(string permission);
}

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal?.FindFirstValue("sub"), out Guid id)
            ? id
            : null;

    public string? Email => Principal?.FindFirstValue(ClaimTypes.Email) ?? Principal?.FindFirstValue("email");

    public IReadOnlySet<string> Permissions =>
        Principal?.FindAll(Security.Permissions.ClaimType).Select(claim => claim.Value).ToHashSet()
        ?? new HashSet<string>();

    public bool HasPermission(string permission) => Permissions.Contains(permission);
}
