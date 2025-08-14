using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a post in the application.
/// </summary>
[Table("PostItems")]
public class PostItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the post.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user who created the post.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the post.
    /// </summary>
    /// <remarks>
    /// This field is initialized to an empty string.
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the post.
    /// </summary>
    /// <remarks>
    /// This field is initialized to an empty string.
    /// </remarks>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the post was created.
    /// </summary>
    public DateTime PostedOn { get; set; }

    /// <summary>
    /// Gets or sets the content of the post.
    /// </summary>
    /// <remarks>
    /// This field is initialized to an empty string.
    /// </remarks>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the thread the post belongs to.
    /// </summary>
    public Guid ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the list of users who favorited the post.
    /// </summary>
    public IEnumerable<Favorite>? Favorites { get; set; }

    /// <summary>
    /// Gets or sets the list of comments on the post.
    /// </summary>
    public IEnumerable<CommentItem>? Comments { get; set; }
}

[Table("Favorites")]
public class Favorite
{
    /// <summary>
    /// Gets or sets the unique identifier for the favorite.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user who favorited the post.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the post that was favorited.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Gets or sets the date and time the post was favorited.
    /// </summary>
    public DateTime FavoritedOn { get; set; }
}