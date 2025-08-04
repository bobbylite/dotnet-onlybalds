using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the posts api.
/// </summary>
public static class PostsEndpoints
{
    private const string ThreadAuthorizationPolicyName = "Thread.ReadWrite";

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
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPost("/posts", CreatePostAsync)
            .WithName(nameof(CreatePostAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPatch("/posts", PatchPostAsync)
            .WithName(nameof(PatchPostAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapDelete("/posts", DeletePostAsync)
            .WithName(nameof(DeletePostAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

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
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            await postsRepository.DeleteById(Guid.Parse(postId));

            return Results.NoContent();
        }

        return Results.BadRequest("Post ID cannot be null or empty.");
    }
}