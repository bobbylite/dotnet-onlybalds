using Microsoft.AspNetCore.Mvc;
using OnlyBalds.Api.Interfaces.Repositories;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Endpoints;

/// <summary>
/// Represents the endpoints for the threads api.
/// </summary>
public static class ThreadsEndpoints
{
    private const string ThreadAuthorizationPolicyName = "Thread.ReadWrite";

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
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapGet("/threads/{id}", GetThreadById)
            .WithName(nameof(GetThreadById))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPost("/threads", CreateThreadAsync)
            .WithName(nameof(CreateThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapPut("/threads/{id}", UpdateThreadAsync)
            .WithName(nameof(UpdateThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        app.MapDelete("/threads/{id}", DeleteThreadAsync)
            .WithName(nameof(DeleteThreadAsync))
            .WithOpenApi()
            .RequireAuthorization(ThreadAuthorizationPolicyName);

        return app;
    }

    /// <summary>
    /// Retrieves all threads from the repository.
    /// </summary>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetThreads([FromServices] IThreadsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(threadsRepository);

        var threads = threadsRepository.GetAll();
        ArgumentNullException.ThrowIfNull(threads);

        return Results.Ok(threads);
    }

    /// <summary>
    /// Retrieves a specific thread by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static IResult GetThreadById(Guid id, [FromServices] IThreadsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(threadsRepository);

        var thread = threadsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(thread);

        return Results.Ok(thread);
    }

    /// <summary>
    /// Creates a new thread.
    /// </summary>
    /// <param name="threadItem"></param>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> CreateThreadAsync([FromBody] ThreadItem threadItem, [FromServices] IThreadsRepository<ThreadItem> threadsRepository)
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
    public static async Task<IResult> UpdateThreadAsync(Guid id, [FromBody] ThreadItem threadItem, [FromServices] IThreadsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(threadItem);
        ArgumentNullException.ThrowIfNull(threadsRepository);

        var thread = threadsRepository.GetById(id);
        ArgumentNullException.ThrowIfNull(thread);

        thread.Title = threadItem.Title;
        thread.PostsCount = threadItem.PostsCount;
        thread.Summary = threadItem.Summary;
        thread.Creator = threadItem.Creator;

        await threadsRepository.UpdateById(id);

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes a thread by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="threadsRepository"></param>
    /// <returns><see cref="IResult"/></returns>
    public static async Task<IResult> DeleteThreadAsync(Guid id, [FromServices] IThreadsRepository<ThreadItem> threadsRepository)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(threadsRepository);

        await threadsRepository.DeleteById(id);

        return Results.NoContent();
    }
}