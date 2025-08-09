using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

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
            // TODO: Push to a list of favorites where favorite contains identity and other data

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

        var accessJwt = httpContextAccessor.HttpContext?.Request.Headers["X-Access"].FirstOrDefault();
        var identityJwt = httpContextAccessor.HttpContext?.Request.Headers["X-Identity"].FirstOrDefault();

        if (string.IsNullOrEmpty(accessJwt) ||
            string.IsNullOrEmpty(identityJwt))
        {
            return Results.Unauthorized();
        }

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var id = Guid.Parse(postId);
            var post = postsRepository.GetById(id);
            ArgumentNullException.ThrowIfNull(post);

            var isAdmin = await IsAuthorizedAdminAsync(accessJwt);
            if (isAdmin is true)
            {
                await postsRepository.DeleteById(id);

                var comments = commentsRepository
                    .GetAll()
                    .Where(c => c.PostId == id)
                    .ToList();

                foreach (var comment in comments)
                {
                    await commentsRepository.DeleteById(comment.Id);
                }

                return Results.NoContent();
            }

            var userId = await GetUserIdAsync(accessJwt);
            if (post.UserId.Equals(userId, StringComparison.InvariantCulture) is true)
            {
                await postsRepository.DeleteById(id);

                var comments = commentsRepository
                    .GetAll()
                    .Where(c => c.PostId == id)
                    .ToList();

                foreach (var comment in comments)
                {
                    await commentsRepository.DeleteById(comment.Id);
                }

                return Results.NoContent();
            }

            return Results.Unauthorized();
        }

        return Results.BadRequest("Post ID cannot be null or empty.");
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

            return permissions.Contains(AuthorizationPolicies.AdminAccess) &&
                scope.Contains(AuthorizationPolicies.AdminAccess);
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