using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Configuration;

/// <summary>
/// Represents the configuration options for the OnlyBalds API.
/// </summary>
/// <remarks>
/// This class represents the configuration options for the OnlyBalds API.
/// </remarks>
public class ApiOptions
{
    /// <summary>
    /// The configuration section key for the OnlyBalds API.
    /// </summary>
    /// <value>The configuration section key for the OnlyBalds API.</value>
    /// <remarks>
    /// This value is used to identify the configuration section for the OnlyBalds API.
    /// </remarks>
    public const string SectionKey = "OnlyBaldsAPI";
    
    /// <summary>
    /// Gets or sets the base URL for the OnlyBalds API.
    /// </summary>
    /// <value>The base URL for the OnlyBalds API.</value>
    /// <remarks>
    /// This value is used to identify the base URL for the OnlyBalds API.
    /// </remarks>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = String.Empty;
}
