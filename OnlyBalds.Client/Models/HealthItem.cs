using System.Text.Json.Serialization;

namespace OnlyBalds.Client.Models;

/// <summary>
/// Represents a health item.
/// </summary>
public class HealthItem
{
    /// <summary>
    /// The status of the health check.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The total duration of the health check.
    /// </summary>
    [JsonPropertyName("totalDuration")]
    public string TotalDuration { get; set; } = string.Empty;

    /// <summary>
    /// The entries for the health check.
    /// </summary>
    [JsonPropertyName("entries")]
    public Dictionary<string, HealthCheckEntry>? Entries { get; set; }
}

/// <summary>
/// Represents a health check entry.
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// The data for the health check entry.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// The duration of the health check entry.
    /// </summary>
    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;

    /// <summary>
    /// The Status for the health check entry.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The tags for the health check entry.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
}