namespace OnlyBalds.Client.Models;

/// <summary>
/// Represents a thread item in a discussion forum or similar platform.
/// </summary>
public class ThreadItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the thread item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the thread item. This field is required.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the count of posts in the thread item.
    /// </summary>
    public int PostsCount { get; set; }

    /// <summary>
    /// Gets or sets the summary of the thread item. This field is required.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creator of the thread item. This field is required.
    /// </summary>
    public string Creator { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the thread item.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the thread item.
    /// </summary>
    /// <remarks>
    /// This field is optional.
    /// </remarks>
    public string Name { get; set; } = string.Empty;
}
