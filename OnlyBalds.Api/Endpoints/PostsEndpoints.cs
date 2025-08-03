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

        app.MapGet("/posts/{id}", GetPostsById)
            .WithName(nameof(GetPostsById))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPost("/posts", CreatePostAsync)
            .WithName(nameof(CreatePostAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPut("/posts/{id}", UpdatePostAsync)
            .WithName(nameof(UpdatePostAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapDelete("/posts/{id}", DeletePostAsync)
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
        [FromQuery] string? postId,
        [FromQuery] string? threadId,
        [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        System.Diagnostics.Debug.WriteLine($"post ID: {postId}");

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var post = postsRepository.GetById(Guid.Parse(postId));
            
            ArgumentNullException.ThrowIfNull(post);
            return Results.Ok(post);
        }

        if (string.IsNullOrEmpty(threadId) is not true)
        {
            var post = postsRepository
                .GetAll()
                .Where(p => p.ThreadId == Guid.Parse(threadId))
                .ToList();

            ArgumentNullException.ThrowIfNull(post);
            return Results.Ok(post);
        }

        var posts = postsRepository.GetAll();
        ArgumentNullException.ThrowIfNull(posts);

        return Results.Ok(posts);
    }

    /// <summary>
    /// Retrieves posts associated with a specific thread.
    /// </summary>
    /// <param name="threadId"></param>
    /// <param name="postsRepository"></param>
    /// <returns></returns>
    public static IResult GetPostsByPostId([FromQuery] string postId, [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        var posts = postsRepository.GetById(Guid.Parse(postId));
        ArgumentNullException.ThrowIfNull(posts);

        return Results.Ok(posts);
    }

    /// <summary>
    /// Retrieves a specific post by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetPostsById(Guid id, [FromServices] IOnlyBaldsRepository<ThreadItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(postsRepository);

        var post = postsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(post);

        return Results.Ok(post);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="postItem"></param>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreatePostAsync([FromBody] PostItem postItem, [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
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
    public static async Task<IResult> UpdatePostAsync(Guid id, [FromBody] PostItem postItem, [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(postItem);
        ArgumentNullException.ThrowIfNull(postsRepository);

        var post = postsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(post);

        post.Title = postItem.Title;
        post.Content = postItem.Content;

        await postsRepository.UpdateById(id);

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a post by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="postsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeletePostAsync(Guid id, [FromServices] IOnlyBaldsRepository<PostItem> postsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(postsRepository);

        await postsRepository.DeleteById(id);

        return Results.NoContent();
    }
}