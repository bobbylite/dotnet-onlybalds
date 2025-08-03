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
            var isAuthenticated = context.User?.Identity?.IsAuthenticated;

            if (isAuthenticated is null || isAuthenticated is false)
            {
                context.Response.ContentType = "text/html";
                context.Response.Redirect("/access-denied.html");

                return Task.CompletedTask;
            }

            context.Response.ContentType = "text/html";
            context.Response.Redirect("/forum.html");

            return Task.CompletedTask;
        }).AllowAnonymous();

        endpoints.MapGet("/forum-new-post/{threadId:guid}", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect($"/forum-new-post.html?threadId={context.Request.RouteValues["threadId"]}");


            return Task.CompletedTask;
        }).RequireAuthorization();

        endpoints.MapGet("/forum-single/{postId:guid}", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect($"/forum-single.html?postId={context.Request.RouteValues["postId"]}");

            return Task.CompletedTask;
        }).RequireAuthorization();

        return endpoints;
    }
}