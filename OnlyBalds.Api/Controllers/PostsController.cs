using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyBalds.Api.Data;

namespace OnlyBalds.Api.Controllers;

/// <summary>
/// A controller for managing posts.
/// </summary>
/// <remarks>
/// This controller provides the ability to create, read, update, and delete posts.
/// </remarks>
[Route("api/v2/[controller]")]
[ApiController]
[Authorize(Policy = "Thread.ReadWrite")]
public class PostsController : ControllerBase
{
    private readonly PostDataContext _dataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostsController"/> class.
    /// </summary>
    /// <param name="dataContext">The data context for the posts.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="PostsController"/> class.
    /// </remarks>
    public PostsController(PostDataContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
    }

    /// <summary>
    /// Gets the posts from the dbcontext.
    /// </summary>
    /// <returns>All posts.</returns>
    [HttpGet("posts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPosts()
    {
        var postItems = await _dataContext.PostItems.ToListAsync();
        return Ok(postItems);
    }

    /// <summary>
    /// Gets the posts by Id specified in the query parameters .
    /// </summary>
    /// <param name="postId"></param>
    /// <returns>Post by Id.</returns>
    [HttpGet("posts/{postId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPostsById([FromQuery] Guid postId)
    {
        ArgumentNullException.ThrowIfNull(postId);

        var postItems = await _dataContext.PostItems.FindAsync(postId);

        if (postItems is null)
        {
            return NotFound();
        }

        return Ok(postItems);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="postItem">The post item to be created.</param>
    /// <returns>The created post item.</returns>
    [HttpPost("posts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePost([FromBody] PostItem postItem)
    {
        ArgumentNullException.ThrowIfNull(postItem);

        if (postItem.Id == Guid.Empty)
        {
            postItem.Id = Guid.NewGuid();
        }

        _dataContext.PostItems.Add(postItem);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction(nameof(CreatePost), new { postId = postItem.Id }, postItem);
    }

    /// <summary>
    /// Updates an existing post by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the post.</param>
    /// <param name="content">The updated post content.</param>
    /// <returns>No content if successful, or not found if the post does not exist.</returns>
    [HttpPut("posts/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] PostItem content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var postItems = await _dataContext.PostItems.FindAsync(id);

        if (postItems is null)
        {
            return NotFound();
        }

        postItems.Title = content.Title;
        postItems.Content = content.Content;
        postItems.PostedOn = content.PostedOn;

        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a post by Id.
    /// </summary>
    /// <param name="id">The unique identifier of the post to delete.</param>
    /// <returns>No content if successful, or not found if the post does not exist.</returns>
    [HttpDelete("posts/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        ArgumentNullException.ThrowIfNull(id);

        var postItems = await _dataContext.PostItems.FindAsync(id);

        if (postItems is null)
        {
            return NotFound();
        }

        _dataContext.PostItems.Remove(postItems);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }
}