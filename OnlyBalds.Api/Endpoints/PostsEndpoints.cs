using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;
using OnlyBalds.Api.Extensions;

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

        app.MapPost("/posts/favorites", AddPostToFavoritesAsync)
            .WithName(nameof(AddPostToFavoritesAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all posts from the repository.
    /// </summary>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetPosts(
        string? postId,
        string? threadId,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var post = postsRepository.GetById(Guid.Parse(postId));

            ArgumentNullException.ThrowIfNull(post);

            return Results.Ok(post);
        }

        if (string.IsNullOrEmpty(threadId) is not true)
        {
            var posts = postsRepository
                .GetAll()
                .Where(c => c.ThreadId == Guid.Parse(threadId))
                .ToList();

            ArgumentNullException.ThrowIfNull(posts);

            return Results.Ok(posts);
        }

        return Results.BadRequest("Post ID and Thread ID cannot be null or empty.");
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

        return Results.Created($"/threads/{postItem.Id}", postItem);
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
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postId);
        ArgumentNullException.ThrowIfNull(postItem);
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var post = postsRepository.GetById(Guid.Parse(postId));
            ArgumentNullException.ThrowIfNull(post);

            post.Title = string.IsNullOrEmpty(postItem.Title) ? post.Title : postItem.Title;
            post.Content = string.IsNullOrEmpty(postItem.Content) ? post.Content : postItem.Content;

            await postsRepository.UpdateById(Guid.Parse(postId));

            return Results.NoContent();
        }

        return Results.BadRequest("Post ID cannot be null or empty.");
    }

    /// <summary>
    /// Deletes post by post id from the repository.
    /// </summary>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeletePostAsync(
        string? postId,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);
        ArgumentNullException.ThrowIfNull(commentsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(postId))
        {
            return Results.BadRequest("Post ID cannot be null or empty.");
        }

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessJwt = httpContext.Request.Headers["X-Access"].FirstOrDefault();
        if (string.IsNullOrEmpty(accessJwt))
        {
            return Results.BadRequest("Missing X-Access header for token validation.");
        }

        var isAuthorized = await httpContext.IsAuthorizedUserAsync(accessJwt);
        var isAuthorizedAdmin = await httpContext.IsAuthorizedAdminAsync(accessJwt);
        var userId = await httpContext.GetUserIdAsync(accessJwt);

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

    private static async Task<IResult> AddPostToFavoritesAsync(
        [FromBody] Favorite favorite,
        [FromServices] IOnlyBaldsRepository<Favorite> favoritesRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(favorite);
        ArgumentNullException.ThrowIfNull(favoritesRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessJwt = httpContext.Request.Headers["X-Access"].FirstOrDefault();
        var identityJwt = httpContext.Request.Headers["X-Identity"].FirstOrDefault();

        if (string.IsNullOrEmpty(accessJwt) ||
            string.IsNullOrEmpty(identityJwt))
        {
            return Results.Unauthorized();
        }

        var userId = await httpContext.GetUserIdAsync(accessJwt);
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        favorite.UserId = userId;
        await favoritesRepository.Add(favorite);

        return Results.Created($"/posts/favorites/{favorite.Id}", favorite);
    }
}