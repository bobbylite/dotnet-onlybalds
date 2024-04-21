using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Configuration;

public class TasksApiOptions
{
    /// <summary>
    /// The section key used to define the options bound to this class (e.g. in appsettings.json).
    /// </summary>
    public const string SectionKey = "TasksAPI";
    
    /// <summary>
    /// The base URL of the Tasks API.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = String.Empty;
}
