﻿using System.Security.Claims;

namespace OnlyBalds.Client.Identity;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
//
// See: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
// See: https://jonhilton.net/blazor-share-auth-state/
public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Name { get; init; }

    private const string UserIdClaimType = "preferred_username";
    private const string NameClaimType = "name";

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal) =>
        new()
        {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
        };

    public ClaimsPrincipal ToClaimsPrincipal() =>
        new(new ClaimsIdentity(
            [new(UserIdClaimType, UserId), new(NameClaimType, Name)],
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: null));

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}