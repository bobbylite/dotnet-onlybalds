﻿using System.ComponentModel.DataAnnotations.Schema;

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
}
