using System.Security.Claims;

namespace OnlyBalds.Client.Identity;

// <summary>
// Represents the user information.
// https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
// https://jonhilton.net/blazor-share-auth-state/
// </summary>
// <remarks>
// This class represents the user information.
// </remarks>
public sealed class UserInfo
{
    // <summary>
    // Gets the user identifier.
    // </summary>
    // <value>The user identifier.</value>
    // <remarks>
    // This field is required.
    // </remarks>
    public required string UserId { get; init; }

    // <summary>
    // Gets the name.
    // </summary>
    // <value>The name.</value>
    // <remarks>
    // This field is required.
    // </remarks>
    public required string Name { get; init; }

    private const string UserIdClaimType = "preferred_username";
    private const string NameClaimType = "name";

    // <summary>
    // Initializes a new instance of the <see cref="UserInfo"/> class.
    // </summary>
    // <remarks>
    // This constructor initializes a new instance of the <see cref="UserInfo"/> class.
    // </remarks>
    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
        };

    // <summary>
    // Converts the user information to a claims principal.
    // </summary>
    // <returns>The claims principal.</returns>
    // <remarks>
    // This method converts the user information to a claims principal.
    // </remarks>
    public ClaimsPrincipal ToClaimsPrincipal() =>
        new(new ClaimsIdentity(
            [new(UserIdClaimType, UserId), new(NameClaimType, Name)],
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: null));

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}