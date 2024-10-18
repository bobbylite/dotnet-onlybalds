using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Data;

namespace OnlyBalds.Api.Controllers;

/// <summary>
/// A controller for managing comments.
/// </summary>
/// <remarks>
/// This controller provides the ability to create, read, update, and delete comments.
/// </remarks>
[Route("api/v2/[controller]")]
[ApiController]
[Authorize(Policy = "Thread.ReadWrite")]
public class CommentsController : ControllerBase
{
    private readonly CommentDataContext _dataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsController"/> class.
    /// </summary>
    /// <param name="dataContext">The data context for the comments.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="CommentsController"/> class.
    /// </remarks>
    public CommentsController(CommentDataContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
    }

    /// <summary>
    /// Gets the comments from the dbcontext.
    /// </summary>
    /// <returns>All comments.</returns>
    [HttpGet("comments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetComments()
    {
        var commentItems = await _dataContext.CommentItems.ToListAsync();
        return Ok(commentItems);
    }

    /// <summary>
    /// Gets the comments by Id specified in the query parameters .
    /// </summary>
    /// <param name="commendIt"></param>
    /// <returns>Comment by Id.</returns>
    [HttpGet("comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCommentsById([FromQuery] Guid commentId)
    {
        ArgumentNullException.ThrowIfNull(commentId);

        var commentItems = await _dataContext.CommentItems.FindAsync(commentId);

        if (commentItems is null)
        {
            return NotFound();
        }

        return Ok(commentItems);
    }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="commentItem">The comment item to be created.</param>
    /// <returns>The created comment item.</returns>
    [HttpPost("comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateComment([FromBody] CommentItem commentItem)
    {
        ArgumentNullException.ThrowIfNull(commentItem);

        if (commentItem.Id == Guid.Empty)
        {
            commentItem.Id = Guid.NewGuid();
        }

        _dataContext.CommentItems.Add(commentItem);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateComment), new { commentId = commentItem.Id }, commentItem);
    }

    /// <summary>
    /// Updates an existing comment by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the comment.</param>
    /// <param name="content">The updated comment content.</param>
    /// <returns>No content if successful, or not found if the comment does not exist.</returns>
    [HttpPut("comments/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CommentItem content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var commentItems = await _dataContext.CommentItems.FindAsync(id);

        if (commentItems is null)
        {
            return NotFound();
        }

        commentItems.Content = content.Content;
        commentItems.PostedOn = content.PostedOn;

        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a comment by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete.</param>
    /// <returns>No content if successful, or not found if the comment does not exist.</returns>
    [HttpDelete("comments/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var commentItems = await _dataContext.CommentItems.FindAsync(id);

        if (commentItems is null)
        {
            return NotFound();
        }

        _dataContext.CommentItems.Remove(commentItems);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }
}

