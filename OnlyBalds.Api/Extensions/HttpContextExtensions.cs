using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OnlyBalds.Api.Constants;

namespace OnlyBalds.Api.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Checks if the user is authorized.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<bool> IsAuthorizedUserAsync(
        this HttpContext httpContext,
        string accessToken
    )
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        var accessTokenClaimsPrincipal = await accessToken.GetClaimsPrincipalAsync(AuthorizationAudiences.AccessToken);

        if (accessTokenClaimsPrincipal == null)
        {
            return false;
        }

        var accessPermissions = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToList();
        ArgumentNullException.ThrowIfNull(accessPermissions);

        var accessScope = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "scope")
                .Select(c => c.Value)
                .SingleOrDefault();
        ArgumentNullException.ThrowIfNullOrEmpty(accessScope);

        var isEmailVerified = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "email_verified")
                .Select(c => c.Value)
                .Contains("true");
        ArgumentNullException.ThrowIfNull(isEmailVerified);

        var containsUserAccessPermission = accessPermissions.Contains(AuthorizationPolicies.UserAccess);
        var containsUserAccessScope = accessScope.Contains(AuthorizationPolicies.UserAccess);

        return containsUserAccessPermission && containsUserAccessScope && isEmailVerified;
    }

    /// <summary>
    /// Checks if the user is an authorized admin.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static async Task<bool> IsAuthorizedAdminAsync(
        this HttpContext httpContext,
        string accessToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        var accessTokenClaimsPrincipal = await accessToken.GetClaimsPrincipalAsync(AuthorizationAudiences.AccessToken);

        if (accessTokenClaimsPrincipal == null)
        {
            return false;
        }

        var accessPermissions = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToList();
        ArgumentNullException.ThrowIfNull(accessPermissions);

        var accessScope = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "scope")
                .Select(c => c.Value)
                .SingleOrDefault();
        ArgumentNullException.ThrowIfNullOrEmpty(accessScope);

        var isEmailVerified = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "email_verified")
                .Select(c => c.Value)
                .Contains("true");
        ArgumentNullException.ThrowIfNull(isEmailVerified);

        var containsUserAccessPermission = accessPermissions.Contains(AuthorizationPolicies.AdminAccess);
        var containsUserAccessScope = accessScope.Contains(AuthorizationPolicies.AdminAccess);

        return containsUserAccessPermission && containsUserAccessScope && isEmailVerified;
    }

    public static async Task<string> GetUserIdAsync(
        this HttpContext httpContext,
        string accessToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNullOrEmpty(accessToken);

        var accessTokenClaimsPrincipal = await accessToken.GetClaimsPrincipalAsync(AuthorizationAudiences.AccessToken);
        ArgumentNullException.ThrowIfNull(accessTokenClaimsPrincipal);

        var userId = accessTokenClaimsPrincipal.Claims
                .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                .Select(c => c.Value)
                .SingleOrDefault();
        ArgumentNullException.ThrowIfNullOrEmpty(userId);

        return userId;
    }
}
