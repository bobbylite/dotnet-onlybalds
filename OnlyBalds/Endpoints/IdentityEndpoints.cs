using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlyBalds.Identity;
using OnlyBalds.Models;

namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds identity endpoints.
/// </summary>
public static class IdentityEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds identity endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints
            .MapGet("/identity-information", GetIdentityInformation)
            .RequireAuthorization();

        endpoints
            .MapGet("/refresh-token", RefreshToken)
            .RequireAuthorization();

        return endpoints;
    }

    private static async Task RefreshToken(
        HttpContext context,
        [FromServices] AccessTokenRefresher refresher,
        [FromServices] IAuthenticationSchemeProvider schemeProvider,
        [FromServices] IOptionsMonitor<CookieAuthenticationOptions> optionsMonitor)
    {
        const string schemeName = CookieAuthenticationDefaults.AuthenticationScheme;

        var scheme = await schemeProvider.GetSchemeAsync(schemeName);
        if (scheme == null)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Authentication scheme not found.");
            return;
        }

        var cookieOptions = optionsMonitor.Get(schemeName);

        var authResult = await context.AuthenticateAsync(schemeName);
        if (!authResult.Succeeded || authResult.Principal == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var ticket = new AuthenticationTicket(authResult.Principal, authResult.Properties, schemeName);
        var cookieContext = new CookieValidatePrincipalContext(
            context,
            scheme,
            cookieOptions,
            ticket
        );

        await refresher.RefreshAccessTokenAsync(cookieContext, "OpenIdConnect");

        if (cookieContext.ShouldRenew)
        {
            await context.SignInAsync(schemeName, cookieContext.Principal!, cookieContext.Properties);
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { refreshed = true });
    }

    private static async Task GetIdentityInformation(
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        var user = context.User;

        if (user?.Identity is not { IsAuthenticated: true })
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                IsAuthenticated = user?.Identity?.IsAuthenticated,
            });

            return;
        }

        var claims = user.Claims
            .ToDictionary(claim => claim.Type, claim => claim.Value);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var subject = context.User?.FindFirst("sub")?.Value;
        var accountsResponse = await httpClient.GetAsync($"account?id={Uri.EscapeDataString(subject!)}");
        AccountItem? account = null;

        if (accountsResponse.StatusCode is HttpStatusCode.NotFound ||
            accountsResponse.StatusCode is HttpStatusCode.NoContent ||
            accountsResponse.StatusCode is HttpStatusCode.Unauthorized ||
            accountsResponse.StatusCode is HttpStatusCode.Forbidden)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Account = account,
                IsAuthenticated = user.Identity.IsAuthenticated,
                Name = user.Identity.Name,
                Claims = claims
            });

            return;
        }

        account = await accountsResponse.Content.ReadFromJsonAsync<AccountItem>();

        if (account is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        await context.Response.WriteAsJsonAsync(new
        {
            Account = account,
            IsAuthenticated = user.Identity.IsAuthenticated,
            Name = user.Identity.Name,
            Claims = claims
        });
    }
}