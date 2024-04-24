using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Configuration;

public class ThreadsApiOptions
{
    /// <summary>
    /// The section key used to define the options bound to this class.
    /// </summary>
    public const string SectionKey = "ThreadsAPI";
    
    /// <summary>
    /// The base URL of the Threads API.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = String.Empty;
}
