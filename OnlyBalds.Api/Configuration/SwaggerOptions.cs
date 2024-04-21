namespace OnlyBalds.Api;

/// <summary>
/// Provides configuration options for Swagger.
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// The key used to retrieve these options from the application's configuration.
    /// </summary>
    public const string SectionKey = "Swagger";
    
    /// <summary>
    /// The URL endpoint where the Swagger JSON is exposed.
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// The name of the API to be displayed on the Swagger UI.
    /// </summary>
    public required string APIName { get; set; }

    /// <summary>
    /// The URL where the OAuth2 provider redirects the user after the authorization has been granted by the user.
    /// </summary>
    public required string OAuth2RedirectUrl { get; set; }

    /// <summary>
    /// The client ID used to identify the application to the OAuth2 provider.
    /// </summary>
    public required string OAuthClientId { get; set; }

    /// <summary>
    /// The client secret used to authenticate the application to the OAuth2 provider.
    /// </summary>
    public required string OAuthClientSecret { get; set; }

    /// <summary>
    /// The realm used with certain OAuth2 providers to partition the OAuth2 scope.
    /// </summary>
    public required string OAuthRealm { get; set; }

    /// <summary>
    /// The name of the application as registered with the OAuth2 provider.
    /// </summary>
    public required string OAuthAppName { get; set; }

    /// <summary>
    /// Specifies whether PKCE should be used with the OAuth2 provider.
    /// </summary>
    public bool OAuthUsePkce { get; set; }

    /// <summary>
    /// The URL where the OAuth2 provider redirects the user to authorize the application.
    /// </summary>
    public required string AuthorizationUrl { get; set;}

    /// <summary>
    /// The URL where the OAuth2 provider redirects the user to authorize the application.
    /// </summary>
    public required string TokenUrl { get; set; }

    /// <summary>
    /// The scopes that the application is requesting access to.
    /// </summary>
    public required Dictionary<string, string> Scopes { get; set; }
}