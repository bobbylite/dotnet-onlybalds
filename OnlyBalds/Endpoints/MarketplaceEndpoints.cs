namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds marketplace endpoints.
/// </summary>
public static class MarketplaceEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds marketplace endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapMarketplaceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/marketplace", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/marketplace.html");

            return Task.CompletedTask;
        }).AllowAnonymous();

        return endpoints;
    }
}