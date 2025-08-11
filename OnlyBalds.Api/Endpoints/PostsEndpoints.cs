using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;
using OnlyBalds.Api.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the posts api.
/// </summary>
public static class PostsEndpoints
{
    /// <summary>
    /// Maps the endpoints for the posts api.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapPostsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/posts", GetPosts)
            .WithName(nameof(GetPosts))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/posts", CreatePostAsync)
            .WithName(nameof(CreatePostAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPatch("/posts", PatchPostAsync)
            .WithName(nameof(PatchPostAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/posts", DeletePostAsync)
            .WithName(nameof(DeletePostAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all posts from the repository.
    /// </summary>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> GetPosts(
        string? postId,
        string? threadId,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var post = await postsRepository
                .GetDbSet()
                .Include(p => p.Favorites)
                .Include(p => p.Comments)
                .Where(p => p.Id == Guid.Parse(postId))
                .SingleOrDefaultAsync();

            ArgumentNullException.ThrowIfNull(post);

            return Results.Ok(post);
        }

        if (string.IsNullOrEmpty(threadId) is not true)
        {
            var posts = await postsRepository
                .GetDbSet()
                .Include(p => p.Favorites)
                .Include(p => p.Comments)
                .Where(c => c.ThreadId == Guid.Parse(threadId))
                .ToListAsync();

            ArgumentNullException.ThrowIfNull(posts);

            return Results.Ok(posts);
        }

        var allPosts = await postsRepository
            .GetDbSet()
            .Include(p => p.Favorites)
            .Include(p => p.Comments)
            .ToListAsync();

        return Results.Ok(allPosts);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="postItem"></param>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreatePostAsync(
        [FromBody] PostItem postItem,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postItem);
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (postItem.Id == Guid.Empty)
        {
            postItem.Id = Guid.NewGuid();
        }

        postItem.PostedOn = DateTime.UtcNow.ToUniversalTime();

        await postsRepository.Add(postItem);

        return Results.Created($"/posts?postId={postItem.Id}", postItem);
    }

    /// <summary>
    /// Updates an existing post with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="postItem"></param>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> PatchPostAsync(
        string? postId,
        [FromBody] PostItem postItem,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(postItem);
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (string.IsNullOrEmpty(postId))
        {
            return Results.BadRequest("Post ID cannot be null or empty.");
        }

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (isAuthorized is false || string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var post = postsRepository.GetById(Guid.Parse(postId));
        ArgumentNullException.ThrowIfNull(post);

        if (post.UserId.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        post.Title = string.IsNullOrEmpty(postItem.Title) ? post.Title : postItem.Title;
        post.Content = string.IsNullOrEmpty(postItem.Content) ? post.Content : postItem.Content;

        await postsRepository.UpdateById(Guid.Parse(postId));

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes post by post id from the repository.
    /// </summary>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeletePostAsync(
        string? postId,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(postId))
        {
            return Results.BadRequest("Post ID cannot be null or empty.");
        }

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.BadRequest("Could not retrieve access token.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessToken);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessToken);
        var userId = await httpContext.GetUserIdAsync(accessToken);

        if (isAuthorized is false || string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var id = Guid.Parse(postId);
        var post = postsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(post);


        if (post.UserId.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await postsRepository.DeleteById(id);

        return Results.NoContent();
    }
}