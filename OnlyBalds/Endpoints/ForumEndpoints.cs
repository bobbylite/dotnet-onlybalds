namespace OnlyBalds.Endpoints;

/// <summary>
/// Provides extension methods for mapping OnlyBalds forum endpoints.
/// </summary>
public static class ForumEndpoints
{
    /// <summary>
    /// Extension methods for mapping OnlyBalds forum endpoints.
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapForumEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/forum", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/forum.html");

            return Task.CompletedTask;
        }).RequireAuthorization();

        endpoints.MapGet("/forum-new-post", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/forum-new-post.html");

            return Task.CompletedTask;
        }).RequireAuthorization();

        endpoints.MapGet("/forum-single", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect("/forum-single.html");

            return Task.CompletedTask;
        }).RequireAuthorization();

        return endpoints;
    }
}