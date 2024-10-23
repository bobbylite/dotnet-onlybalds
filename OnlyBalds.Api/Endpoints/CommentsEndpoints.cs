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

        app.MapGet("/comments/{id}", GetCommentsById)
            .WithName(nameof(GetCommentsById))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPost("/comments", CreateCommentAsync)
            .WithName(nameof(CreateCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPut("/comments/{id}", UpdateCommentAsync)
            .WithName(nameof(UpdateCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapDelete("/comments/{id}", DeleteCommentAsync)
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
    public static IResult GetComments([FromServices] ICommentsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);

        var comments = commentsRepository.GetAll();
        ArgumentNullException.ThrowIfNull(comments);

        return Results.Ok(comments);
    }

    /// <summary>
    /// Retrieves a specific comment by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetCommentsById(Guid id, [FromServices] ICommentsRepository<CommentItem> commentsRepository)
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
    public static async Task<IResult> CreateCommentAsync([FromBody] CommentItem commentItem, [FromServices] ICommentsRepository<CommentItem> commentsRepository)
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
    public static async Task<IResult> UpdateCommentAsync(Guid id, [FromBody] CommentItem commentItem, [FromServices] ICommentsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(commentItem);
        ArgumentNullException.ThrowIfNull(commentsRepository);

        var comment = commentsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(comment);

        comment.Content = commentItem.Content;

        await commentsRepository.UpdateById(id);

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a comment by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteCommentAsync(Guid id, [FromServices] ICommentsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(commentsRepository);

        await commentsRepository.DeleteById(id);

        return Results.NoContent();
    }
}
