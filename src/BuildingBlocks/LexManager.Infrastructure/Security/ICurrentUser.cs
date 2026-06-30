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
