using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;
using OnlyBalds.Api.Extensions;
using Microsoft.AspNetCore.Authentication;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the favorites api.
/// </summary>
public static class FavoritesEndpoints
{
    /// <summary>
    /// Maps the endpoints for the favorites api.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapFavoritesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/favorites", GetFavorites)
            .WithName(nameof(GetFavorites))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/favorites", CreateFavoritesAsync)
            .WithName(nameof(CreateFavoritesAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/favorites", DeleteFavoriteAsync)
            .WithName(nameof(DeleteFavoriteAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all favorites from the repository.
    /// </summary>
    /// <param name="favoritesRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetFavorites(
        string? favoriteId,
        string? postId,
        [FromServices] IOnlyBaldsRepository<Favorite> favoritesRepository)
    {
        ArgumentNullException.ThrowIfNull(favoritesRepository);

        if (string.IsNullOrEmpty(favoriteId) is not true)
        {
            var favorite = favoritesRepository.GetById(Guid.Parse(favoriteId));
            ArgumentNullException.ThrowIfNull(favorite);

            return Results.Ok(favorite);
        }

        if (string.IsNullOrEmpty(postId) is not true)
        {
            var posts = favoritesRepository
                .GetAll()
                .Where(f => f.PostId == Guid.Parse(postId))
                .ToList();

            ArgumentNullException.ThrowIfNull(posts);

            return Results.Ok(posts);
        }

        return Results.Ok(favoritesRepository.GetAll());
    }

    /// <summary>
    /// Creates a new favorite for a post.
    /// </summary>
    /// <param name="favorite"></param>
    /// <param name="favoritesRepository"></param>
    /// <param name="httpContextAccessor"></param>
    /// <returns></returns>
    public static async Task<IResult> CreateFavoritesAsync(
        [FromBody] Favorite favorite,
        [FromServices] IOnlyBaldsRepository<Favorite> favoritesRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(favorite);
        ArgumentNullException.ThrowIfNull(favoritesRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.Unauthorized();
        }

        var userId = await httpContext.GetUserIdAsync(accessToken);
        if (string.IsNullOrEmpty(userId))
        {
            return Results.Unauthorized();
        }

        var favorites = favoritesRepository
            .GetAll()
            .Where(f => f.Id == favorite.PostId)
            .ToList();

        var existingFavorite = favorites
            .Where(f => f.UserId == userId)
            .ToList();

        if (existingFavorite.Count is > 0)
        {
            return Results.Conflict("User has already liked this post.");
        }

        await favoritesRepository.Add(favorite);

        return Results.Created($"/favorites?favoriteId={favorite.Id}", favorite);
    }

    /// <summary>
    /// Deletes a favorite by favorite id from the repository.
    /// </summary>
    /// <param name="favoritesRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteFavoriteAsync(
        string? favoriteId,
        [FromServices] IOnlyBaldsRepository<Favorite> favoritesRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(favoritesRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(favoriteId))
        {
            return Results.BadRequest("Favorite ID cannot be null or empty.");
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

        var id = Guid.Parse(favoriteId);
        var favorite = favoritesRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(favorite);


        if (favorite.UserId.ToString().Equals(userId, StringComparison.OrdinalIgnoreCase) is false)
        {
            if (isAuthorizedAdmin is false)
            {
                return Results.Unauthorized();
            }
        }

        await favoritesRepository.DeleteById(id);

        return Results.NoContent();
    }
}