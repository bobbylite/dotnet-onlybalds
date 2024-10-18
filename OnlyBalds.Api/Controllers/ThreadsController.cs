using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Data;
using OnlyBalds.Api.Models;

namespace OnlyBalds.Api.Controllers;

/// <summary>
/// A controller for managing threads.
/// </summary>
/// <remarks>
/// This controller provides the ability to create, read, update, and delete threads.
/// </remarks>
[Route("api/v2/[controller]")]
[ApiController]
[Authorize(Policy = "Thread.ReadWrite")]
public class ThreadsController : ControllerBase
{
    private readonly ThreadDataContext _dataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadsController"/> class.
    /// </summary>
    /// <param name="dataContext">The data context for the threads.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ThreadsController"/> class.
    /// </remarks>
    public ThreadsController(ThreadDataContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
    }

    /// <summary>
    /// Gets the threads from the dbcontext.
    /// </summary>
    /// <returns>All threads.</returns>
    [HttpGet("threads")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetThreads()
    {
        var threadItems = await _dataContext.ThreadItems.ToListAsync();
        return Ok(threadItems);
    }

    /// <summary>
    /// Gets the threads by Id specified in the query parameters .
    /// </summary>
    /// <param name="threadId"></param>
    /// <returns>Thread by Id.</returns>
    [HttpGet("threads/{threadId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetThreadsById([FromQuery] Guid threadId)
    {
        ArgumentNullException.ThrowIfNull(threadId);

        var threadItems = await _dataContext.ThreadItems.FindAsync(threadId);

        if (threadItems is null)
        {
            return NotFound();
        }

        return Ok(threadItems);
    }

    /// <summary>
    /// Creates a new thread.
    /// </summary>
    /// <param name="threadItem">The thread item to be created.</param>
    /// <returns>The created thread item.</returns>
    [HttpPost("threads")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateThread([FromBody] ThreadItem threadItem)
    {
        ArgumentNullException.ThrowIfNull(threadItem);

        if (threadItem.Id == Guid.Empty)
        {
            threadItem.Id = Guid.NewGuid();
        }

        _dataContext.ThreadItems.Add(threadItem);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateThread), new { threadId = threadItem.Id }, threadItem);
    }

    /// <summary>
    /// Updates an existing thread by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the thread.</param>
    /// <param name="content">The updated thread content.</param>
    /// <returns>No content if successful, or not found if the thread does not exist.</returns>
    [HttpPut("threads/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateThread(Guid id, [FromBody] ThreadItem content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var threadItem = await _dataContext.ThreadItems.FindAsync(id);

        if (threadItem is null)
        {
            return NotFound();
        }

        threadItem.Name = content.Name;
        threadItem.Title = content.Title;
        threadItem.Summary = content.Summary;
        threadItem.Creator = content.Creator;
        threadItem.StartDate = content.StartDate;
        threadItem.PostsCount = content.PostsCount;

        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a thread by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the thread to delete.</param>
    /// <returns>No content if successful, or not found if the thread does not exist.</returns>
    [HttpDelete("threads/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteThread(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var threadItem = await _dataContext.ThreadItems.FindAsync(id);

        if (threadItem is null)
        {
            return NotFound();
        }

        _dataContext.ThreadItems.Remove(threadItem);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }
}