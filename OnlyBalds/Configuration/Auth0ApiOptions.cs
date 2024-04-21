namespace OnlyBalds;

/// <summary>
/// Represents the configuration options for Auth0 authentication.
/// </summary>
public class Auth0ApiOptions
{
    /// <summary>
    /// The section key used to define the options bound to this class (e.g. in appsettings.json).
    /// </summary>
    public const string SectionKey = "Auth0Api";

    /// <summary>
    /// Gets or sets the endpoint for the Auth0 authorization token.
    /// This is the URL where your application will send a request to get an authorization token.
    /// </summary>
    public string AuthorizationTokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client ID for the Auth0 application.
    /// This is a unique identifier for your Auth0 application, provided by Auth0.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret for the Auth0 application.
    /// This is a secret key, provided by Auth0, which is used to secure communication between your application and the Auth0 authentication server.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base url for the Auth0 application.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience for the Auth0 application.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}


