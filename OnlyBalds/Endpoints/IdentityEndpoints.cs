using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Client.Models;

namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds identity endpoints.
/// </summary>
public static class IdentityEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds identity endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints
        .MapGet("/identity-information", GetIdentityInformation)
        .RequireAuthorization();

        return endpoints;
    }

    private static async Task GetIdentityInformation(
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var subject = context.User?.FindFirst("sub")?.Value;
        var response = await httpClient.GetFromJsonAsync<AccountItem>($"account?id={Uri.EscapeDataString(subject!)}");

        if (response is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var user = context.User;

        if (user?.Identity is not { IsAuthenticated: true })
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var claims = user.Claims
            .ToDictionary(claim => claim.Type, claim => claim.Value);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Account = response,
            IsAuthenticated = user.Identity.IsAuthenticated,
            Name = user.Identity.Name,
            Claims = claims
        });
    }
}