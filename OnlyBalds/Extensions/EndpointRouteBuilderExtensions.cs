using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace OnlyBalds.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
/// <remarks>
/// This class provides extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </remarks>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps the login and logout endpoints.
    /// </summary>
    /// <param name="endpointRouteBuilder">The endpoint route builder.</param>
    /// <returns>The endpoint convention builder.</returns>
    /// <remarks>
    /// This method maps the login and logout endpoints.
    /// </remarks>
    public static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        ArgumentNullException.ThrowIfNull(endpointRouteBuilder);

        var group = endpointRouteBuilder.MapGroup("");

        group.MapGet("/login", (string? returnUrl) =>
                TypedResults.Challenge(GetAuthProperties(returnUrl)))
            .AllowAnonymous();

        // To get oidc configuration working with RIP Initiated logout, Auth0 tenant must be configured: https://auth0.com/docs/authenticate/login/logout/log-users-out-of-auth0
        group.MapPost("/logout",
            ([FromForm] string? returnUrl,
                    [FromServices] IOptionsSnapshot<AuthenticationOptions> authenticationOptions) =>
                TypedResults.SignOut(GetAuthProperties(returnUrl),
                    [CookieAuthenticationDefaults.AuthenticationScheme, authenticationOptions.Value.DefaultAuthenticateScheme!]));
        
        return group;
    }

    private static AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        // Prevent open redirects.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = pathBase;
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}