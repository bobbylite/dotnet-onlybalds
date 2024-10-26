using System.ComponentModel.DataAnnotations;

namespace OnlyBalds.Configuration;

/// <summary>
/// Options for the Inference API.
/// </summary>
public class InferenceApiOptions
{

    /// <summary>
    /// The key for the section in the configuration file.
    /// </summary>
    /// <remarks>
    /// This value is used to bind the configuration file to the options.
    /// </remarks>
    public const string SectionKey = "InferenceApiOptions";
    
    /// <summary>
    /// The base URL for the Inference API.
    /// </summary>
    [Url]
    public string BaseUrl { get; set; } = String.Empty;

    /// <summary>
    /// The API key for the Inference API.
    /// </summary>
    public string? ApiKey { get; set; }
}
