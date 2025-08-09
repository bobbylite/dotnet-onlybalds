namespace OnlyBalds.Api.Constants;

/// <summary>
/// Constants representing the names of authorization claims.
/// </summary>
public static class AuthorizationPermissions
{
    /// <summary>
    /// The name of the authorization claim for standard user access.
    /// </summary>
    public const string UserAccess = "user:access";

    /// <summary>
    /// The name of the authorization claim for admin access.
    /// </summary>
    public const string AdminAccess = "admin:access";

    /// <summary>
    /// The name of the authorization claim for email verification status.
    /// </summary>
    public const string EmailIsVerified = "true";
}