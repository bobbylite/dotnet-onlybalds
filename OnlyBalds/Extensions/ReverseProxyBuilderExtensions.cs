using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yarp.ReverseProxy.Transforms;

namespace OnlyBalds.Extensions;

public static class ReverseProxyBuilderExtensions
{
    /// <summary>
    /// Adds the BFF reverse proxy to the application.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder AddBffReverseProxy(this IReverseProxyBuilder builder, IConfiguration configuration, IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);
        
        var oidcOptions = configuration
            .GetSection("Auth0Api")
            .Get<Auth0ApiOptions>();
        ArgumentNullException.ThrowIfNull(oidcOptions);

        builder.AddTransforms(context =>
        {
            context.AddRequestTransform(async transformContext =>
            {
                var result = await transformContext.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (result.Succeeded is false)
                {
                    transformContext.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var emailVerified = transformContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
                var idToken = await transformContext.HttpContext.GetTokenAsync("id_token");
                var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");

                transformContext.ProxyRequest.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var originalUri = transformContext.Path.Value;
                var updatedPath = originalUri?.Replace("/onlybalds-api", "", StringComparison.OrdinalIgnoreCase);
                var queryString = transformContext.Query.QueryString.Value;
                transformContext.ProxyRequest.RequestUri = new Uri($"{transformContext.DestinationPrefix}{updatedPath}{queryString}");
                transformContext.ProxyRequest.Headers.Add("X-Access", accessToken);
                transformContext.ProxyRequest.Headers.Add("X-Identity", idToken);
                transformContext.ProxyRequest.Headers.Add("X-User-Email-Verified", emailVerified);
            });
        });

        return builder;
    }
}