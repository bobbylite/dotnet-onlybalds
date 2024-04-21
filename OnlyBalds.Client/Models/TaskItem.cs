/// <summary>
/// The OnlyBalds.Client.Models namespace contains models used in the OnlyBalds client application.
/// </summary>
namespace OnlyBalds.Client.Models
{
    /// <summary>
    /// Represents a task in the application.
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the task. The default value is an empty string.
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the task is complete.
        /// </summary>
        public bool IsComplete { get; set; }
    }
}