using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Configuration;

/// <summary>
/// Options for configuring the API.
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// The section key used to define the options bound to this class.
    /// </summary>
    public const string SectionKey = "OnlyBaldsAPI";
    
    /// <summary>
    /// The base URL of the Threads API.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = String.Empty;
}
