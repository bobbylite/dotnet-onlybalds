using System.Text.Json.Serialization;

namespace OnlyBalds.Models;

// <summary>
// Represents the response from the token endpoint
// </summary>
public class TokenResponse
{

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the expires in.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }
}