using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Flurl;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

                var idToken = await transformContext.HttpContext.GetTokenAsync("id_token");
                var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
                
                var httpClientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
                using var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBaldsAuthenticationToken);

                var requestBody = new
                {
                    client_id = oidcOptions.ClientId,
                    client_secret = oidcOptions.ClientSecret,
                    grant_type = "client_credentials",
                    audience = oidcOptions.Audience
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var tokenUrl = $"{oidcOptions.BaseUrl.TrimEnd('/')}/oauth/token";
                var response = await httpClient.PostAsync(tokenUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Response: {errorResponse}");
                }

                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var apiAccessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

                transformContext.ProxyRequest.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiAccessToken);

                var originalUri = transformContext.Path.Value;
                var updatedPath = originalUri?.Replace("/onlybalds-api", "", StringComparison.OrdinalIgnoreCase);
                transformContext.ProxyRequest.RequestUri = new Uri($"{transformContext.DestinationPrefix}{updatedPath}");

                if (transformContext.ProxyRequest.Method.Method == HttpMethod.Get.Method)
                {
                    return;
                }

                if (transformContext.ProxyRequest.Method.Method == HttpMethod.Post.Method)
                {
                    var proxyRequestjson = await transformContext.ProxyRequest.Content?.ReadAsStringAsync()!;
                    var proxyRequestJsonDoc = JsonDocument.Parse(proxyRequestjson);
                    var todoName = proxyRequestJsonDoc.RootElement.GetProperty("name").GetString();
                    var todoDate = proxyRequestJsonDoc.RootElement.GetProperty("date").GetString();
                    var identityName = transformContext.HttpContext.User.Claims.Single(c => c.Type == "nickname").Value;
                    var profilePicture = transformContext.HttpContext.User.Claims.Single(c => c.Type == "picture").Value;
                    var payload = new
                    {
                        name = todoName,
                        date = todoDate,
                        createdBy = identityName,
                        profilePicture = profilePicture
                    };

                    var payloadContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, MediaTypeNames.Application.Json);
                    transformContext.ProxyRequest.Content = payloadContent;
                }
            });
        });

        return builder;
    }
}