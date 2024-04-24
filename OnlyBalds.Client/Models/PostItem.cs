namespace OnlyBalds.Client;

/// <summary>
/// Represents a post in the application.
/// </summary>
public class PostItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the post.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the post. This field is initialized to an empty string.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the post. This field is initialized to an empty string.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the post was created.
    /// </summary>
    public DateTime PostedOn { get; set; }

    /// <summary>
    /// Gets or sets the content of the post. This field is initialized to an empty string.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    // TODO: Add a property for the thread the post belongs to.
}
