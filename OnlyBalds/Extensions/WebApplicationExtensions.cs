using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using OnlyBalds.Configuration;
using OnlyBalds.Hubs;
using OnlyBalds.Services.Token;
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
    /// Maps endpoints for proxying requests from WASM to the Threads API.
    /// 
    /// https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
    /// </summary>
    /// <param name="webApplication">The web application used to configure the HTTP pipeline, and routes.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static WebApplication MapThreadsApiProxy(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        // Enable support for proxying / forwarding requests from the WebAssembly client to the Tasks API.
        var apiOptionsMonitor = webApplication.Services.GetRequiredService<IOptionsMonitor<ApiOptions>>();

        // The following is for doing direct proxying with Yarp.
        // https://microsoft.github.io/reverse-proxy/articles/direct-forwarding.html
        // https://satish1v.medium.com/yarp-based-direct-forwarding-pattern-f3f7d556be4b
        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false,
            ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current)
        });

        var requestOptions = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };
        webApplication.Map("/onlybalds-api/{**catch-all}",
                async (HttpContext context, IHttpForwarder httpForwarder, ITransformBuilder transform) =>
                {
                    var transformer = transform.Create((builderContext) =>
                    {
                        builderContext
                            .AddRequestTransform(async transformContext =>
                            {
                                var apiTokenService = webApplication.Services.GetRequiredService<ITokenService>();
                                await apiTokenService.AuthenticateAsync();
                                var accessToken = apiTokenService.Token;
                                transformContext.ProxyRequest.Headers.Authorization =
                                    new AuthenticationHeaderValue("Bearer", accessToken);
                            })
                            .AddPathRemovePrefix(prefix: "/onlybalds-api");
                    });
                    
                    var error = await httpForwarder.SendAsync(context, apiOptionsMonitor.CurrentValue.BaseUrl,
                        httpClient, requestOptions, transformer);
                    
                    var forwarderErrorFeature = context.GetForwarderErrorFeature();
                    if (error != ForwarderError.None
                        && forwarderErrorFeature is not null)
                    {
                        var exception = forwarderErrorFeature.Exception;
                        webApplication.Logger.LogError(exception, "An error occurred proxying the request");
                    }
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
