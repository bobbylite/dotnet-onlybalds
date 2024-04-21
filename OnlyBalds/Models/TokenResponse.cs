using System.Text.Json.Serialization;

namespace OnlyBalds.Models;

/// <summary>
/// Represents the response from an OAuth 2.0 token endpoint for the client credentials grant type flow.
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Gets or sets the access token issued by the authorization server.
    /// This token is used to access the protected resources.
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the type of the token issued.
    /// Typically, this is "bearer".
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the lifetime in seconds of the access token.
    /// For example, an "expires_in" value of 3600 indicates that the access token will expire in one hour from the time the response was generated.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the scope of the access token.
    /// This is a list of the scopes that the client application is allowed to access.
    /// </summary>
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }
}