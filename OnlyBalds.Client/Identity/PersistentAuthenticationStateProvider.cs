using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace OnlyBalds.Client.Identity;

// <summary>
// Represents the persistent authentication state provider.
// https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
// https://jonhilton.net/blazor-share-auth-state/
// </summary>
// <remarks>
// This class represents the persistent authentication state provider.
// </remarks>

internal sealed class PersistentAuthenticationStateProvider(PersistentComponentState persistentState) : AuthenticationStateProvider
{
  private static readonly Task<AuthenticationState> _unauthenticatedTask =
      Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

  // <summary>
  // Initializes a new instance of the <see cref="PersistentAuthenticationStateProvider"/> class using the specified persistent state.
  // </summary>
  // <param name="persistentState">The persistent state.</param>
  // <remarks>
  // This constructor initializes a new instance of the <see cref="PersistentAuthenticationStateProvider"/> class using the specified persistent state.
  // </remarks>
  public override Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    if (!persistentState.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
    {
      return _unauthenticatedTask;
    }

    return Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));
  }
}