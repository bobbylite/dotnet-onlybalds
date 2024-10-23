using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a thread item.
/// </summary>
/// <remarks>
/// This class represents a thread item.
/// </remarks>
[Table("ThreadItems")]
public class ThreadItem
{
    /// <summary>
    /// Gets or sets the identifier of the thread item.
    /// </summary>
    /// <value>The identifier of the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the thread item. This field is required.
    /// </summary>
    /// <value>The title of the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the count of posts in the thread item.
    /// </summary>
    /// <value>The count of posts in the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public int PostsCount { get; set; }

    /// <summary>
    /// Gets or sets the summary of the thread item.
    /// </summary>
    /// <value>The summary of the thread item.</value>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creator of the thread item.
    /// </summary>
    /// <value>The creator of the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public string Creator { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the thread item.
    /// </summary>
    /// <value>The start date of the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the thread item.
    /// </summary>
    /// <value>The end date of the thread item.</value>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    public string Name { get; set; } = string.Empty; // TODO: delete me
}