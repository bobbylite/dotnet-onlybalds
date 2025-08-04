using System.Text.Json.Serialization;

/// <summary>
/// Represents the payload of an access token.
/// </summary>
public class IdentityPayload
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The audience for which the token is intended.
    /// </summary>
    [JsonPropertyName("audience")]
    public List<string> Audience { get; set; } = new();

    /// <summary>
    /// The scope of the access token, indicating the permissions granted.
    /// </summary>
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// A list of permissions granted to the user.
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new();
}
