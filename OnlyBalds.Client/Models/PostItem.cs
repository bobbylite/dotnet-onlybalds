using System.Text.Json.Serialization;

namespace OnlyBalds.Client.Models;

/// <summary>
/// Represents a post in the application.
/// </summary>
public class PostItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the post.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the post. This field is initialized to an empty string.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of the post. This field is initialized to an empty string.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the post was created.
    /// </summary>
    [JsonPropertyName("postedOn")]
    public DateTime PostedOn { get; set; }

    /// <summary>
    /// Gets or sets the content of the post. This field is initialized to an empty string.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the thread the post belongs to.
    /// </summary>
    [JsonPropertyName("threadId")]
    public Guid ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user who created the post.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
}
