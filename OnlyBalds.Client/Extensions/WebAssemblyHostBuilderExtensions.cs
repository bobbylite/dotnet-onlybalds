using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlyBalds.Client.Identity;

namespace OnlyBalds.Client.Extensions;


/// <summary>
/// Extension methods for <see cref="WebAssemblyHostBuilderExtensions"/>.
/// </summary>
public static class WebAssemblyHostBuilderExtensions
{
    /// <summary>
    /// Add an HttpClient for the Threads REST API.
    /// </summary>
    /// <param name="webAssemblyHostBuilder">A builder for WebAssembly applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebAssemblyHostBuilder AddOnlyBaldsApiClients(this WebAssemblyHostBuilder webAssemblyHostBuilder)
    {
        ArgumentNullException.ThrowIfNull(webAssemblyHostBuilder);

        webAssemblyHostBuilder
        .Services
        .AddHttpClient(HttpClientNames.OnlyBalds, client =>
        {
            // Do include trailing slash - see https://stackoverflow.com/a/23438417
            client.BaseAddress = new Uri($"{webAssemblyHostBuilder.HostEnvironment.BaseAddress}onlybalds-api/");
            Console.WriteLine("BaseAddress = " + client.BaseAddress);
        });
        
        return webAssemblyHostBuilder;
    }
    
    /// <summary>
    /// Add support for authentication and authorization to the application.
    /// </summary>
    /// <param name="webAssemblyHostBuilder">A builder for WebAssembly applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebAssemblyHostBuilder AddAccessControl(this WebAssemblyHostBuilder webAssemblyHostBuilder)
    {
        ArgumentNullException.ThrowIfNull(webAssemblyHostBuilder);

        webAssemblyHostBuilder.Services.AddAuthorizationCore();
        webAssemblyHostBuilder.Services.AddCascadingAuthenticationState();
        webAssemblyHostBuilder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
        
        return webAssemblyHostBuilder;
    }
}
