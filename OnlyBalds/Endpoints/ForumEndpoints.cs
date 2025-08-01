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

        return MapApiEndpoints(endpoints);
    }

    private static IEndpointRouteBuilder MapApiEndpoints(IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/threads", GetThreads).RequireAuthorization();
        endpoints.MapGet("/thread", GetThread).RequireAuthorization();
        endpoints.MapPost("/threads", PostThread).RequireAuthorization();
        endpoints.MapGet("/articles", GetPosts).RequireAuthorization();
        endpoints.MapGet("/article", GetPost).RequireAuthorization();
        endpoints.MapPost("/articles", PostArticle).RequireAuthorization();
        endpoints.MapGet("/article-comments", GetComments).RequireAuthorization();
        endpoints.MapPost("/article-comments", PostComment).RequireAuthorization();

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
        var posts = response?.Where(p => p.ThreadId == Guid.Parse(threadId!)).ToList();


        if (posts is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Posts not found for the specified thread.");
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(posts);
    }

    private static async Task GetPost(
    HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] string? postId = null
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        if (string.IsNullOrWhiteSpace(postId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Missing threadId query parameter.");
            return;
        }

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.GetFromJsonAsync<List<PostItem>>($"posts");
        var post = response?.SingleOrDefault(p => p.Id == Guid.Parse(postId!));

        if (post is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Thread not found.");
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(post);
    }

    private static async Task PostArticle(
        [FromBody] PostItem postItem,
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(postItem);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.PostAsJsonAsync("posts", postItem);

        if (response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync("Post created successfully.");
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Failed to create post.");
        }
    }

    private static async Task PostThread(
        [FromBody] ThreadItem threadItem,
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(threadItem);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.PostAsJsonAsync("threads", threadItem);

        if (response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync("Thread created successfully.");
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Failed to create thread.");
        }
    }

    private static async Task GetComments(
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromQuery] string? postId = null
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var commentsReponse = await httpClient.GetFromJsonAsync<List<CommentItem>>($"comments");
        var comments = commentsReponse?.Where(c => c.PostId == Guid.Parse(postId!)).ToList();

        if (comments is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Comments not found.");
            return;
        }

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(comments);
    }

    private static async Task PostComment(
        [FromBody] CommentItem commentItem,
        HttpContext context,
        [FromServices] IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(commentItem);

        var httpClient = httpClientFactory.CreateClient(HttpClientNames.OnlyBalds);
        var response = await httpClient.PostAsJsonAsync("comments", commentItem);

        if (response.IsSuccessStatusCode)
        {
            context.Response.StatusCode = StatusCodes.Status201Created;
            await context.Response.WriteAsync("Comment created successfully.");
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Failed to create comment.");
        }
    }
}