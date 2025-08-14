using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Constants;
using OnlyBalds.Api.Extensions;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Endpoints for the threads api.
/// </summary>
public static class ThreadsEndpoints
{
    /// <summary>
    /// Maps the endpoints for the threads api.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapThreadsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/threads", GetThreads)
            .WithName(nameof(GetThreads))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPost("/threads", CreateThreadAsync)
            .WithName(nameof(CreateThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapPatch("/threads", PatchThreadAsync)
            .WithName(nameof(PatchThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        app.MapDelete("/threads", DeleteThreadAsync)
            .WithName(nameof(DeleteThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(AuthorizationPolicies.UserAccess);

        return app;
    }

    /// <summary>
    /// Retrieves all threads from the repository.
    /// </summary>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetThreads(
        string? threadId,
        [FromServices] IOnlyBaldsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(threadsRepository);

        if (string.IsNullOrEmpty(threadId) is not true)
        {
            var thread = threadsRepository.GetById(Guid.Parse(threadId));

            return Results.Ok(thread);
        }

        return Results.Ok(threadsRepository.GetAll());
    }

    /// <summary>
    /// Creates a new thread.
    /// </summary>
    /// <param name="threadItem"></param>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateThreadAsync(
        [FromBody] ThreadItem threadItem,
        [FromServices] IOnlyBaldsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(threadItem);
        ArgumentNullException.ThrowIfNull(threadsRepository);

        if (threadItem.Id == Guid.Empty)
        {
            threadItem.Id = Guid.NewGuid();
        }

        threadItem.StartDate = DateTime.UtcNow.ToUniversalTime();

        await threadsRepository.Add(threadItem);

        return Results.Created($"/threads/{threadItem.Id}", threadItem);
    }

    /// <summary>
    /// Updates an existing thread with new data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="threadItem"></param>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> PatchThreadAsync(
        string? threadId,
        [FromBody] ThreadItem threadItem,
        [FromServices] IOnlyBaldsRepository<ThreadItem> threadsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(threadItem);
        ArgumentNullException.ThrowIfNull(threadsRepository);

        if (string.IsNullOrEmpty(threadId))
        {
            return Results.BadRequest("Thread ID cannot be null or empty.");
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

        var thread = threadsRepository.GetById(Guid.Parse(threadId));
        ArgumentNullException.ThrowIfNull(thread);

        if (isAuthorizedAdmin is false)
        {
            return Results.Unauthorized();
        }

        thread.Summary = string.IsNullOrEmpty(threadItem.Summary) ? thread.Summary : threadItem.Summary;
        thread.Title = string.IsNullOrEmpty(threadItem.Title) ? thread.Title : threadItem.Title;
        thread.PostsCount = threadItem.PostsCount > 0 ? threadItem.PostsCount : thread.PostsCount;

        await threadsRepository.UpdateById(Guid.Parse(threadId));

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes thread by thread id from the repository.
    /// </summary>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteThreadAsync(
        string? threadId,
        [FromServices] IOnlyBaldsRepository<ThreadItem> threadsRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(threadsRepository);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        if (string.IsNullOrEmpty(threadId))
        {
            return Results.BadRequest("Thread ID cannot be null or empty.");
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

        var id = Guid.Parse(threadId);
        var thread = threadsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(thread);

        if (isAuthorizedAdmin is false)
        {
            return Results.Unauthorized();
        }

        await threadsRepository.DeleteById(id);

        return Results.NoContent();
    }
}
