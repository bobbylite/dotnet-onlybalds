namespace OnlyBalds.Api.Constants;

/// <summary>
/// Constants representing the names of authorization claims.
/// </summary>
public static class AuthorizationClaims
{
    /// <summary>
    /// The name of the authorization claim for standard user access.
    /// </summary>
    public const string Permissions = "permissions";

    /// <summary>
    /// The name of the authorization claim for email verification status.
    /// </summary>
    public const string EmailVerified = "email_verified";
}