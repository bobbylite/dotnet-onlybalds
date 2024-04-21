using OnlyBalds.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using OnlyBalds.Http;
using OnlyBalds.Identity;
using Ardalis.GuardClauses;
using OnlyBalds.Services.Token;

namespace OnlyBalds.Extensions;

public static class WebApplicationBuilderExtensions
{   
    /// <summary>
    /// Adds services to the specified <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="webApplicationBuilder">The <see cref="WebApplicationBuilder"/> to add services to.</param>
    /// <returns>The same <see cref="WebApplicationBuilder"/> so that multiple calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="WebApplicationBuilder"/> is null.</exception>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder webApplicationBuilder)
    { 
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        // Add a singleton service of type ITokenService. The same instance of TokenService will be used every time ITokenService is requested.
        webApplicationBuilder.Services.AddSingleton<ITokenService, TokenService>();
        webApplicationBuilder.Services.AddHttpContextAccessor();

        return webApplicationBuilder;
    }

    /// <summary>
    /// Add an HttpClient for the Tasks REST API. Includes handling authentication for the API by passing the access
    /// token for the currently logged in user as a JWT bearer token to the API.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplicationBuilder AddTasksApiClient(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);
        
        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<TasksApiOptions>()
            .BindConfiguration(TasksApiOptions.SectionKey);

        webApplicationBuilder.Services.AddTransient<TasksApiAuthenticationHandler>();
        webApplicationBuilder.Services.AddHttpClient(HttpClientNames.TasksApi, (provider, client) =>
        {
            var apiOptionsSnapshot = provider.GetRequiredService<IOptionsMonitor<TasksApiOptions>>();
            var apiOptions = apiOptionsSnapshot.CurrentValue;
            client.BaseAddress = new Uri(apiOptions.BaseUrl);
        })
            .AddHttpMessageHandler<TasksApiAuthenticationHandler>();
        
        return webApplicationBuilder;
    }

    /// <summary>
    /// Add an HttpClient for the Tasks REST API. Includes handling authentication for the API by passing the access
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
    /// Add support for authentication and authorization to the application.
    ///
    /// With .NET 8, the configuration for this can be controled completely from 'appsettings.json'.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
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
