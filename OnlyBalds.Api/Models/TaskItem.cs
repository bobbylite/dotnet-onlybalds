using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a task item in the application.
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the task item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the task item. This field is required.
    /// </summary>
    [Required]
    public string Name { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the task item is complete.
    /// </summary>
    public bool IsComplete { get; set; }
}