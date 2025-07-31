using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a comment in the application.
/// </summary>
[Table("CommentItems")]
public class CommentItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the comment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user who created the comment.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the comment.
    /// </summary>
    /// <remarks>
    /// This field is initialized to an empty string.
    /// </remarks>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the comment was posted.
    /// </summary>
    public DateTime PostedOn { get; set; }

    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    /// <remarks>
    /// This field is initialized to an empty string.
    /// </remarks>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the post that this comment is associated with.
    /// </summary>
    public Guid PostId { get; set; }
}