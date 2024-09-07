using OnlyBalds.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using OnlyBalds.Http;
using OnlyBalds.Identity;
using OnlyBalds.Services.Token;
using Yarp.ReverseProxy.Forwarder;

namespace OnlyBalds.Extensions;

public static class WebApplicationBuilderExtensions
{   
    /// <summary>
    /// Add services to the application.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds services to the application.
    /// </remarks>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder webApplicationBuilder)
    { 
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddServiceDiscovery();

        webApplicationBuilder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });
        
        // Add a singleton service of type ITokenService. The same instance of TokenService will be used every time ITokenService is requested.
        webApplicationBuilder.Services.AddSingleton<ITokenService, TokenService>();
        webApplicationBuilder.Services.AddHttpContextAccessor();
        webApplicationBuilder.Services.AddSignalR();

        return webApplicationBuilder;
    }

    /// <summary>
    /// Add an HttpClient for the OnlyBalds REST API. Includes handling authentication for the API by passing the access
    /// token for the currently logged in user as a JWT bearer token to the API.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed of type <see cref="WebApplicationBuilder"/></returns>
    /// <remarks>
    /// This method adds an HttpClient for the OnlyBalds REST API.
    /// </remarks>
    public static WebApplicationBuilder AddOnlyBaldsApiClients(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);
        
        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<ApiOptions>()
            .BindConfiguration(ApiOptions.SectionKey);

        webApplicationBuilder.Services.AddTransient<OnlyBaldsApiAuthenticationHandler>()
        .AddHttpClient(HttpClientNames.OnlyBalds, (provider, client) =>
        {
            var apiOptionsSnapshot = provider.GetRequiredService<IOptionsMonitor<ApiOptions>>();
            var apiOptions = apiOptionsSnapshot.CurrentValue;
            client.BaseAddress = new Uri($"https+http://{apiOptions.BaseUrl}"); 
        })
        .AddHttpMessageHandler<OnlyBaldsApiAuthenticationHandler>();

        webApplicationBuilder.Services.AddKeyedSingleton<IForwarderHttpClientFactory, ApiForwarderFactory>(nameof(ApiForwarderFactory));
        
        return webApplicationBuilder;
    }

    /// <summary>
    /// Add an HttpClient for the OnlyBalds REST API. Includes handling authentication for the API by passing the access
    /// token for the currently logged in user as a JWT bearer token to the API.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddTokenClient(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);
        
        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<Auth0ApiOptions>()
            .BindConfiguration(Auth0ApiOptions.SectionKey);

        webApplicationBuilder.Services.AddTransient<AuthenticationHandler>();
        webApplicationBuilder.Services.AddHttpClient(HttpClientNames.OnlyBaldsAuthenticationToken, (provider, client) =>
        {
            var apiOptionsSnapshot = provider.GetRequiredService<IOptionsMonitor<Auth0ApiOptions>>();
            var apiOptions = apiOptionsSnapshot.CurrentValue;
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        })
            .AddHttpMessageHandler<AuthenticationHandler>();
        
        return webApplicationBuilder;
    }

    /// <summary>
    /// Add support for refreshing access tokens using refresh tokens.
    /// With .NET 8, the configuration for this can be controled completely from 'appsettings.json'.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds support for refreshing access tokens using refresh tokens.
    /// </remarks>
    public static WebApplicationBuilder AddAccessControl(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        // Authentication with OpenID Connect: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
        webApplicationBuilder.Services.AddAuthentication("OpenIdConnect")
            .AddOpenIdConnect()
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        webApplicationBuilder.Services.AddAuthorization();
        webApplicationBuilder.Services.AddRefreshTokenSupport();
        webApplicationBuilder.Services.AddCascadingAuthenticationState();
        
        // Add support for flowing the server authentication state to the WebAssembly client.
        // Sync authentication state between server and client: https://auth0.com/blog/auth0-authentication-blazor-web-apps/
        webApplicationBuilder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        
        return webApplicationBuilder;
    }
}
