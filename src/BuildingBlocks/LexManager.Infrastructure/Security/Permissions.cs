namespace LexManager.Infrastructure.Security;

/// <summary>
/// Fine-grained RBAC permissions (SRD §5.1). Endpoints opt into a permission with
/// <c>.RequireAuthorization(Permissions.Xxx)</c>; a matching authorization policy is registered
/// for every constant here. Permissions are carried as <c>permission</c> claims on the JWT.
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";

    public const string ClientsRead = "clients:read";
    public const string ClientsWrite = "clients:write";
    public const string CasesRead = "cases:read";
    public const string CasesWrite = "cases:write";
    public const string DocumentsRead = "documents:read";
    public const string DocumentsWrite = "documents:write";
    public const string DocumentsReadConfidential = "documents:read-confidential";
    public const string CalendarRead = "calendar:read";
    public const string CalendarWrite = "calendar:write";
    public const string TimeWrite = "time:write";
    public const string BillingRead = "billing:read";
    public const string BillingWrite = "billing:write";
    public const string AdminManage = "admin:manage";

    public static IReadOnlyList<string> All { get; } =
    [
        ClientsRead, ClientsWrite, CasesRead, CasesWrite,
        DocumentsRead, DocumentsWrite, DocumentsReadConfidential,
        CalendarRead, CalendarWrite, TimeWrite,
        BillingRead, BillingWrite, AdminManage
    ];
}
