using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Extensions;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the comments api.
/// </summary>
public static class CommentsEndpoints
{
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
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/comments", CreateCommentAsync)
            .WithName(nameof(CreateCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPatch("/comments", PatchCommentAsync)
            .WithName(nameof(PatchCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/comments", DeleteCommentAsync)
            .WithName(nameof(DeleteCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all comments from the repository.
    /// </summary>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetComments(
        string? commentId,
        string? postId,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);

        if (string.IsNullOrEmpty(commentId) is not true)
        {
            var comment = commentsRepository.GetById(Guid.Parse(commentId));

            return Results.Ok(comment);
        }

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var posts = commentsRepository
                .GetAll()
                .Where(c => c.PostId == Guid.Parse(postId))
                .ToList();

            ArgumentNullException.ThrowIfNull(posts);

            return Results.Ok(posts);
        }

        return Results.Ok(commentsRepository.GetAll());
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
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(commentItem);
        ArgumentNullException.ThrowIfNull(commentsRepository);

        if (string.IsNullOrEmpty(commentId))
        {
            return Results.BadRequest("Comment ID cannot be null or empty.");
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

        var comment = commentsRepository.GetById(Guid.Parse(commentId));
        ArgumentNullException.ThrowIfNull(comment);

        if (comment.UserId.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        if (string.IsNullOrEmpty(commentItem.Content))
        {
            return Results.BadRequest("Comment content cannot be empty.");
        }

        comment.Content =  commentItem.Content;
        await commentsRepository.UpdateById(Guid.Parse(commentId));

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes comment by comment id from the repository.
    /// </summary>
    /// <param name="commentsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteCommentAsync(
        string? commentId,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(commentId))
        {
            return Results.BadRequest("Comment ID cannot be null or empty.");
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

        if (string.IsNullOrEmpty(userId) || isAuthorized is false)
        {
            return Results.Unauthorized();
        }

        var id = Guid.Parse(commentId);
        var comment = commentsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(comment);

        if (comment.UserId.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await commentsRepository.DeleteById(id);

        return Results.NoContent();
    }
}
