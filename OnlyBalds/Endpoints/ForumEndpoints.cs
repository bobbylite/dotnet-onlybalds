using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Client.Models;

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

        endpoints.MapGet("/forum-single/{threadId:guid}", context =>
        {
            context.Response.ContentType = "text/html";
            context.Response.Redirect($"/forum-single.html?threadId={context.Request.RouteValues["threadId"]}");

            return Task.CompletedTask;
        }).RequireAuthorization();

        return MapApiEndpoints(endpoints);
    }

    private static IEndpointRouteBuilder MapApiEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/threads", GetThreads).RequireAuthorization();
        endpoints.MapGet("/thread", GetThread).RequireAuthorization();
        endpoints.MapGet("/articles", GetPosts).RequireAuthorization();

        return endpoints;
    }

    private static async Task GetThread(
    HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] string? threadId = null
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        if (string.IsNullOrWhiteSpace(threadId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing threadId query parameter.");
            return;
        }

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);

        try
        {
            var response = await httpClient.GetFromJsonAsync<ThreadItem>($"threads/{threadId}");

            if (response is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Thread not found.");
                return;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (HttpRequestException ex)
        {
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsync($"Failed to fetch thread: {ex.Message}");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync($"Server error: {ex.Message}");
        }
    }

    private static async Task GetThreads(
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.GetFromJsonAsync<List<ThreadItem>>("threads");

        if (response is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task GetPosts(
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] string? threadId = null
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.GetFromJsonAsync<List<PostItem>>($"posts");
        //posts = postsClient.Where(p => p.ThreadId == Guid.Parse(ThreadId)).ToList();


        if (response is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Posts not found for the specified thread.");
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }   
}