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

        endpoints.MapGet("/identity-information", async context =>
        {
            var user = context.User;

            if (user?.Identity is not { IsAuthenticated: true })
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var claims = user.Claims
                .ToDictionary(claim => claim.Type, claim => claim.Value);

            await context.Response.WriteAsJsonAsync(new
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                Name = user.Identity.Name,
                Claims = claims
            });
        }).RequireAuthorization();

        return endpoints;
    }
}