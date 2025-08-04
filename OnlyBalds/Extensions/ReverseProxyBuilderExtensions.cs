using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Flurl;
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

                var idToken = await transformContext.HttpContext.GetTokenAsync("id_token");
                var accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");

                /*var httpClientFactory = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
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
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiAccessToken);*/

                transformContext.ProxyRequest.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var originalUri = transformContext.Path.Value;
                var updatedPath = originalUri?.Replace("/onlybalds-api", "", StringComparison.OrdinalIgnoreCase);
                var queryString = transformContext.Query.QueryString.Value;
                transformContext.ProxyRequest.RequestUri = new Uri($"{transformContext.DestinationPrefix}{updatedPath}{queryString}");
                transformContext.ProxyRequest.Headers.Add("X-Access", accessToken);
                transformContext.ProxyRequest.Headers.Add("X-Identity", idToken);

                /*var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);

                foreach (var claim in jwt.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                var subject = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var audience = jwt.Claims.Where(c => c.Type == "aud").Select(c => c.Value).ToList();
                var scope = jwt.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;
                var permissions = jwt.Claims.Where(c => c.Type == "permissions").Select(c => c.Value).ToList();*/

                if (transformContext.ProxyRequest.Method.Method == HttpMethod.Get.Method)
                {
                    return;
                }

                if (transformContext.ProxyRequest.Method.Method == HttpMethod.Post.Method)
                {
                    return;
                }
            });
        });

        return builder;
    }
}