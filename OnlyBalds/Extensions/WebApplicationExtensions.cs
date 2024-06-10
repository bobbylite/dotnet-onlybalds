
using Microsoft.Extensions.Options;
using OnlyBalds.Configuration;
using OnlyBalds.Http;
using OnlyBalds.Hubs;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace OnlyBalds.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplication"/>.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps endpoints for proxying requests from WASM to the Threads REST API.
    ///
    /// This is based on the example found here:
    ///
    /// * https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
    /// </summary>
    /// <param name="webApplication">The web application used to configure the HTTP pipeline, and routes.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplication MapThreadsApiProxy(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        
        const string prefix = "/onlybalds-api";
        webApplication.Map(prefix + "/{**catch-all}",
                async (HttpContext context, IHttpForwarder httpForwarder, ITransformBuilder transform) =>
                {
                    var transformer = transform.Create(builderContext =>
                    {
                        builderContext.AddPathRemovePrefix(prefix: prefix);
                    });
                    
                    var optionsSnapshot = context.RequestServices.GetRequiredService<IOptionsSnapshot<ApiOptions>>();
                    var factory = context.RequestServices.GetRequiredKeyedService<IForwarderHttpClientFactory>(nameof(ApiForwarderFactory));
                    var httpClient = factory.CreateClient(new ForwarderHttpClientContext
                    {
                        NewConfig = HttpClientConfig.Empty
                    });

                    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
                    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
                    await httpForwarder.SendAsync(context, $"https+http://{optionsSnapshot.Value.BaseUrl}", 
                        httpClient, ForwarderRequestConfig.Empty, transformer);
                })
            .RequireAuthorization();

        return webApplication;
    }

    public static WebApplication MapChatHub(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapHub<ChatHub>("/chathub");

        return webApplication;
    }
}
