using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the comments api.
/// </summary>
public static class CommentsEndpoints
{
    private const string ThreadAuthorizationPolicyName = "Thread.ReadWrite";

    /// <summary>
    /// Maps the endpoints for the comments api.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapCommentsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/comments", GetComments)
            .WithName(nameof(GetComments))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPost("/comments", CreateCommentAsync)
            .WithName(nameof(CreateCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPatch("/comments", PatchCommentAsync)
            .WithName(nameof(PatchCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapDelete("/comments", DeleteCommentAsync)
            .WithName(nameof(DeleteCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        return app;
    }

    /// <summary>
    /// Retrieves all comments from the repository.
    /// </summary>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetComments(
        string? postId,
        string? commentId,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var comment = commentsRepository
                .GetAll()
                .Where(c => c.PostId == Guid.Parse(postId))
                .ToList();

            ArgumentNullException.ThrowIfNull(comment);

            return Results.Ok(comment);
        }

        if (string.IsNullOrEmpty(commentId) is not true)
        {
            var comment = commentsRepository.GetById(Guid.Parse(commentId));
            ArgumentNullException.ThrowIfNull(comment);

            return Results.Ok(comment);
        }

        return Results.BadRequest("Post ID and Comment ID cannot be null or empty.");
    }

    /// <summary>
    /// Retrieves a specific comment by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetCommentsById(Guid id, [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);

        var comment = commentsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(comment);

        return Results.Ok(comment);
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="commentItem"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateCommentAsync(
        [FromBody] CommentItem commentItem,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentItem);
        ArgumentNullException.ThrowIfNull(commentsRepository);

        if (commentItem.Id == Guid.Empty)
        {
            commentItem.Id = Guid.NewGuid();
        }

        commentItem.PostedOn = DateTime.UtcNow.ToUniversalTime();
        
        await commentsRepository.Add(commentItem);

        return Results.Created($"/threads/{commentItem.Id}", commentItem);
    }

    /// <summary>
    /// Updates an existing comment with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentItem"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> PatchCommentAsync(
        string? commentId,
        [FromBody] CommentItem commentItem,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentId);
        ArgumentNullException.ThrowIfNull(commentItem);
        ArgumentNullException.ThrowIfNull(commentsRepository);

        var comment = commentsRepository.GetById(Guid.Parse(commentId));
        ArgumentNullException.ThrowIfNull(comment);

        comment.Content = commentItem.Content;

        await commentsRepository.UpdateById(Guid.Parse(commentId));

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a comment by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteCommentAsync(
        string? postId,
        string? commentId,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var comments = commentsRepository
                .GetAll()
                .Where(c => c.PostId == Guid.Parse(postId))
                .ToList();

            if (comments.Count == 0)
            {
                return Results.NotFound("Comments not found.");
            }
            
            foreach (var comment in comments)
            {
                await commentsRepository.DeleteById(comment.Id);
            }

            return Results.NoContent();
        }

        if (string.IsNullOrEmpty(commentId) is not true)
        {
            var id = Guid.Parse(commentId);
            var comment = commentsRepository.GetById(id);
            ArgumentNullException.ThrowIfNull(comment);

            await commentsRepository.DeleteById(id);

            return Results.NoContent();
        }

        return Results.BadRequest("Post ID and Comment ID cannot be null or empty.");
    }
}
