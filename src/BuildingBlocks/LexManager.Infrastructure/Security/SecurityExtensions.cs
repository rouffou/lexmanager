using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace LexManager.Infrastructure.Security;

public static class SecurityExtensions
{
    /// <summary>
    /// Registers OAuth2/OIDC JWT authentication, the <see cref="ICurrentUser"/> accessor, and a
    /// permission-based authorization policy per <see cref="Permissions"/> entry (SRD §5.1 RBAC).
    /// Works with an external IdP (set <c>Authentication:Authority</c>) or, for local/dev tokens,
    /// a symmetric <c>Authentication:SigningKey</c>.
    /// </summary>
    public static IServiceCollection AddLexManagerSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICurrentUser, CurrentUser>();

        IConfigurationSection auth = configuration.GetSection("Authentication");
        string? authority = auth["Authority"];
        string? audience = auth["Audience"];
        string? issuer = auth["Issuer"];
        string? signingKey = auth["SigningKey"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = !bool.TryParse(auth["RequireHttpsMetadata"], out bool requireHttps) || requireHttps;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(authority) || !string.IsNullOrEmpty(issuer),
                    ValidIssuer = issuer ?? authority,
                    ValidateAudience = !string.IsNullOrEmpty(audience),
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = !string.IsNullOrEmpty(signingKey),
                    IssuerSigningKey = string.IsNullOrEmpty(signingKey)
                        ? null
                        : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (string permission in Permissions.All)
            {
                options.AddPolicy(permission, policy => policy
                    .RequireAuthenticatedUser()
                    .RequireClaim(Permissions.ClaimType, permission));
            }
        });

        return services;
    }
}
