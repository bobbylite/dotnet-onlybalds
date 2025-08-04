using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OnlyBalds.Api.Constants;
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
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapPost("/comments", CreateCommentAsync)
            .WithName(nameof(CreateCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapPatch("/comments", PatchCommentAsync)
            .WithName(nameof(PatchCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

        app.MapDelete("/comments", DeleteCommentAsync)
            .WithName(nameof(DeleteCommentAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizataionPolicyNames.UserAccess);

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
        string? commentId,
        [FromServices] IOnlyBaldsRepository<CommentItem> commentsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(commentsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        var accessJwt = httpContextAccessor.HttpContext?.Request.Headers["X-Access"].SingleOrDefault();
        var identityJwt = httpContextAccessor.HttpContext?.Request.Headers["X-Identity"].SingleOrDefault();

        if (string.IsNullOrEmpty(accessJwt) ||
            string.IsNullOrEmpty(identityJwt))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrEmpty(commentId) is not true)
        {
            var id = Guid.Parse(commentId);
            var comment = commentsRepository.GetById(id);
            ArgumentNullException.ThrowIfNull(comment);

            var isAdmin = await IsAuthorizedAdminAsync(accessJwt);
            if (isAdmin is true)
            {
                await commentsRepository.DeleteById(id);
                return Results.NoContent();
            }

            var userId = await GetUserIdAsync(accessJwt);
            if (comment.UserId.Equals(userId, StringComparison.InvariantCulture) is true)
            {
                await commentsRepository.DeleteById(id);
                return Results.NoContent();
            }

            return Results.Unauthorized();
        }

        return Results.BadRequest("Comment ID cannot be null or empty.");
    }

    private static async Task<bool> IsAuthorizedAdminAsync(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        var issuer = "https://onlybalds.us.auth0.com/";
        var audience = "https://OnlyBaldsBackendForFrontendsApi";

        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{issuer}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever()
        );

        var config = await configurationManager.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKeys = config.SigningKeys,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(accessToken, validationParameters, out var validatedToken);

            var permissions = principal.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value)
                .ToList();

            var scope = principal.FindFirst("scope")?.Value ?? string.Empty;

            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            return permissions.Contains(AuthorizataionPolicyNames.AdminAccess) &&
                scope.Contains(AuthorizataionPolicyNames.AdminAccess);
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }
    }

    private static async Task<string> GetUserIdAsync(string accessToken)
    {
        var issuer = "https://onlybalds.us.auth0.com/";
        var audience = "https://OnlyBaldsBackendForFrontendsApi";

        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"{issuer}.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever()
        );

        var config = await configurationManager.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKeys = config.SigningKeys,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var principal = handler.ValidateToken(accessToken, validationParameters, out var validatedToken);

            var userId = principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId ?? string.Empty;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return string.Empty;
        }
    }
}
