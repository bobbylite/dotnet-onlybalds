using OnlyBalds.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;

namespace OnlyBalds.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add support for refreshing access tokens using refresh tokens.
    /// 
    /// See: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidc
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="cookieScheme">The name of the cookie authentication scheme.</param>
    /// <param name="oidcScheme">The name of the OIDC authentication scheme.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddRefreshTokenSupport(this IServiceCollection services, 
        string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme, string oidcScheme = "OpenIdConnect")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(cookieScheme);
        ArgumentException.ThrowIfNullOrWhiteSpace(oidcScheme);
        
        services.AddSingleton<AccessTokenRefresher>();
        services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<AccessTokenRefresher>((cookieOptions, refresher) =>
        {
            cookieOptions.Events.OnValidatePrincipal = context => refresher.RefreshAccessTokenAsync(context, oidcScheme);
        });
        
        services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
        {
            // Request a refresh_token.
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            // Store the refresh_token.
            oidcOptions.SaveTokens = true;
        });
        
        return services;
    }
}
