using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
        [FromServices] IOptionsMonitor<OpenIdConnectOptions> openIdConnectOptionsMonitor
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(openIdConnectOptionsMonitor);

        var oidcOptions = openIdConnectOptionsMonitor.Get(OpenIdConnectDefaults.AuthenticationScheme);
        var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        ArgumentNullException.ThrowIfNull(authResult?.Properties);

        var refreshToken = await context.GetTokenAsync("refresh_token");

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", OpenIdConnectGrantTypes.RefreshToken },
            { "refresh_token", refreshToken! },
            { "client_id", oidcOptions.ClientId ?? string.Empty },
            { "client_secret", oidcOptions.ClientSecret ?? string.Empty }
        };

        var tokenEndpoint = $"{oidcOptions.Authority}/oauth/token";
        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

            ArgumentNullException.ThrowIfNull(tokenResponse?.AccessToken);
            ArgumentNullException.ThrowIfNull(tokenResponse?.RefreshToken);
            ArgumentNullException.ThrowIfNull(tokenResponse?.IdToken);

            authResult.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authResult.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            authResult.Properties.UpdateTokenValue("id_token", tokenResponse.IdToken);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenResponse.IdToken);

            var newClaims = jwtToken.Claims.ToList();

            var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var newPrincipal = new ClaimsPrincipal(newIdentity);

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, newPrincipal, authResult.Properties);

            context.User = newPrincipal;
        }

        context.Response.ContentType = MediaTypeNames.Application.Json;
        var user = context.User;
        var claims = user.Claims
            .ToDictionary(claim => claim.Type, claim => claim.Value);

        await context.Response.WriteAsJsonAsync(new
        {
            IsAuthenticated = user.Identity?.IsAuthenticated,
            Name = user.Identity?.Name,
            Claims = claims,
            refreshed = true 
        });
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