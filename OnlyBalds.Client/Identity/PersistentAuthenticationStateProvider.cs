using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace OnlyBalds.Client.Identity;

// This is a client-side AuthenticationStateProvider that uses PersistentComponentState to flow the authentication state to the client.
// See: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
// See: https://jonhilton.net/blazor-share-auth-state/
internal sealed class PersistentAuthenticationStateProvider(PersistentComponentState persistentState) : AuthenticationStateProvider
{
  private static readonly Task<AuthenticationState> _unauthenticatedTask =
      Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

  public override Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    if (!persistentState.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
    {
      return _unauthenticatedTask;
    }

    return Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));
  }
}