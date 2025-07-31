
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.FileProviders;
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
/// Provides extension methods for <see cref="WebApplication"/>.
/// </summary>
/// <remarks>
/// This class provides extension methods for <see cref="WebApplication"/>.
/// </remarks>
public static class WebApplicationExtensions
{

    /// <summary>
    /// Adds the access control services to the application.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseOnlyBaldsAccessControl(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app
        .UseRouting()
        .UseAuthentication()
        .UseAuthorization();

        return app;
    }

    /// <summary>
    /// Adds the BFF reverse proxy to the application.
    /// Adds Back-End For Front-End (BFF) reverse proxy services to the application.
    /// </summary>
    /// <remarks>
    /// The BFF reverse proxy is a reverse proxy that is used to forward requests from the client to the back-end services.
    /// More information on the BFF pattern can be found at 
    // https://learn.microsoft.com/en-us/azure/architecture/patterns/backends-for-frontends
    /// </remarks>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseBffReverseProxy(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet("/auth/login", async (HttpContext context, string? returnUrl = "/") =>
        {
            var redirectUri = returnUrl ?? "/";
            var authProps = new AuthenticationProperties { RedirectUri = redirectUri };
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authProps);
        }).AllowAnonymous();

        app.MapGet("/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return Results.SignOut();
        });

        app.MapGet("/auth/user", async (HttpContext context) =>
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded is false)
            {
                return Results.Unauthorized();
            }

            return Results.Json(new
            {
                context.User?.Identity?.Name,
                Claims = context.User?.Claims.Select(c => new { c.Type, c.Value })
            });
        }).RequireAuthorization();
        
        app.MapReverseProxy();

        return app;
    }
    /// <summary>
    /// Adds the access control services to the application.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseStaticFilesOnClient(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var currentPath = Directory.GetCurrentDirectory();
        var clientPublishPath = Path.Combine(currentPath, "wwwroot");        
        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "index.html" },
            FileProvider = new PhysicalFileProvider(clientPublishPath)
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(clientPublishPath)
        });

        app.UseStaticFiles();

        return app;
    }

    /// <summary>
    /// Maps the OnlyBalds API proxy.
    /// This is based on the example found here:
    /// * https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
    /// </summary>
    /// <param name="webApplication">The web application used to configure the HTTP pipeline, and routes.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method maps the OnlyBalds API proxy.
    /// </remarks>
    public static WebApplication MapOnlyBaldsApiProxy(this WebApplication webApplication)
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

    /// <summary>
    /// Maps the chat hub.
    /// </summary>
    /// <param name="webApplication">The web application used to configure the HTTP pipeline, and routes.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method maps the chat hub.
    /// </remarks>
    public static WebApplication MapChatHub(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapHub<ChatHub>("/chathub").RequireAuthorization();

        return webApplication;
    }
}
