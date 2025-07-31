using OnlyBalds.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OnlyBalds.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>
/// This class provides extension methods for <see cref="IServiceCollection"/>.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the access control services to the application.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOnlyBaldsAccessControl(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        var oidcOptions = configuration
            .GetSection("Authentication:Schemes:OpenIdConnect")
            .Get<OpenIdConnectOptions>();
        ArgumentNullException.ThrowIfNull(oidcOptions);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "__Host-auth";
            options.Cookie.SameSite = SameSiteMode.Strict;
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = oidcOptions.Authority;
            options.ClientId = oidcOptions.ClientId;
            options.ClientSecret = oidcOptions.ClientSecret;
            options.ResponseType = "code";
            options.ResponseMode = "query";

            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.SaveTokens = true;
            options.DisableTelemetry = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.Scope.Add("user:access");

            options.TokenValidationParameters = new()
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return services;
    }
    
    /// <summary>
    /// Add support for refreshing access tokens using refresh tokens.
    /// https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidc
    /// </summary>
    /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
    /// <param name="cookieScheme">The name of the cookie authentication scheme.</param>
    /// <param name="oidcScheme">The name of the OIDC authentication scheme.</param>
    /// <returns>The service collection.</returns>
    /// <remarks>
    /// This method adds support for refreshing access tokens using refresh tokens.
    /// </remarks>
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
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            oidcOptions.SaveTokens = true;
        });
        
        return services;
    }
}
