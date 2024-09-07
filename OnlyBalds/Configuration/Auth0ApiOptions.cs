namespace OnlyBalds;

/// <summary>
/// Represents the configuration options for the Auth0 API.
/// </summary>
/// <remarks>
/// This class represents the configuration options for the Auth0 API.
/// </remarks>
public class Auth0ApiOptions
{
    /// <summary>
    /// The configuration section key for the Auth0 API.
    /// </summary>
    /// <value>The configuration section key for the Auth0 API.</value>
    /// <remarks>
    /// This value is used to identify the configuration section for the Auth0 API.
    /// </remarks>
    public const string SectionKey = "Auth0Api";

    /// <summary>
    /// Gets or sets the authorization token endpoint for the Auth0 application.
    /// This is the URL used to request an authorization token from the Auth0 authentication server.
    /// </summary>
    /// <value>The authorization token endpoint for the Auth0 application.</value>
    /// <remarks>
    /// This value is used to request an authorization token from the Auth0 authentication server.
    /// </remarks>
    public string AuthorizationTokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client id for the Auth0 application.
    /// This is a public key, provided by Auth0, which is used to identify your application to the Auth0 authentication server.
    /// </summary>
    /// <value>The client id for the Auth0 application.</value>
    /// <remarks>
    /// This value is used to identify your application to the Auth0 authentication server.
    /// </remarks>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret for the Auth0 application.
    /// This is a private key, provided by Auth0, which is used to authenticate your application to the Auth0 authentication server.
    /// </summary>
    /// <value>The client secret for the Auth0 application.</value>
    /// <remarks>
    /// This value is used to authenticate your application to the Auth0 authentication server.
    /// </remarks>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the Auth0 application.
    /// </summary>
    /// <value>The base URL for the Auth0 application.</value>
    /// <remarks>
    /// This value is used to identify the base URL for the Auth0 application.
    /// </remarks>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience for the Auth0 application.
    /// </summary>
    /// <value>The audience for the Auth0 application.</value>
    /// <remarks>
    /// This value is used to identify the audience for the Auth0 application.
    /// </remarks>
    public string Audience { get; set; } = string.Empty;
}


