using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using OnlyBalds.Client.Identity;

namespace OnlyBalds.Identity;

// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
//
// See: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
// See: https://jonhilton.net/blazor-share-auth-state/
internal sealed class PersistingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly PersistentComponentState _state;
  private readonly IdentityOptions _options;

  private readonly PersistingComponentStateSubscription _subscription;

  private Task<AuthenticationState>? _authenticationStateTask;

  /// <summary>
  /// Initializes a new instance of the <see cref="PersistingAuthenticationStateProvider"/> class.
  /// </summary>
  /// <param name="loggerFactory">The logger factory.</param>
  /// <param name="scopeFactory">The service scope factory.</param>
  /// <param name="state">The persistent component state.</param>
  /// <param name="options">The identity options.</param>
  public PersistingAuthenticationStateProvider(
      ILoggerFactory loggerFactory,
      IServiceScopeFactory scopeFactory,
      PersistentComponentState state,
      IOptions<IdentityOptions> options)
      : base(loggerFactory)
  {
    _scopeFactory = scopeFactory;
    _state = state;
    _options = options.Value;

    AuthenticationStateChanged += OnAuthenticationStateChanged;
    _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
  }

  /// <summary>
  /// Gets the revalidation interval.
  /// </summary>
  protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

  /// <summary>
  /// Validates the authentication state asynchronously.
  /// </summary>
  /// <param name="authenticationState">The authentication state.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A value indicating whether the authentication state is valid.</returns>
  protected override async Task<bool> ValidateAuthenticationStateAsync(
      AuthenticationState authenticationState, CancellationToken cancellationToken)
  {
    // Get the user manager from a new scope to ensure it fetches fresh data
    await using var scope = _scopeFactory.CreateAsyncScope();
    return ValidateSecurityStampAsync(authenticationState.User);
  }

  /// <summary>
  /// Validates the security stamp of the claims principal.
  /// </summary>
  /// <param name="principal">The claims principal.</param>
  /// <returns>A value indicating whether the claims principal is authenticated.</returns>
  private bool ValidateSecurityStampAsync(ClaimsPrincipal principal)
  {
    if (principal.Identity?.IsAuthenticated is false)
    {
      return false;
    }
    return true;
  }

  /// <summary>
  /// Handles the authentication state changed event.
  /// </summary>
  /// <param name="authenticationStateTask">The authentication state task.</param>
  private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
  {
    _authenticationStateTask = authenticationStateTask;
  }

  /// <summary>
  /// Handles the persisting event asynchronously.
  /// </summary>
  /// <returns>A task that represents the asynchronous operation.</returns>
  private async Task OnPersistingAsync()
  {
    if (_authenticationStateTask is null)
    {
      throw new UnreachableException($"Authentication state not set in {nameof(RevalidatingServerAuthenticationStateProvider)}.{nameof(OnPersistingAsync)}().");
    }

    var authenticationState = await _authenticationStateTask;
    var principal = authenticationState.User;

    if (principal.Identity?.IsAuthenticated == true)
    {
      var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
      var name = principal.FindFirst("name")?.Value;
      var email = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
      var nickname = principal.FindFirst("nickname")?.Value;
      var subject = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

      if (userId != null && name != null && email != null && nickname != null && subject != null)
      {
        _state.PersistAsJson(nameof(UserInfo), new UserInfo
        {
          UserId = userId,
          Name = name,
          NickName = nickname,
          Email = email,
          Subject = subject
        });
      }
    }
  }

  /// <summary>
  /// Disposes the resources used by this instance.
  /// </summary>
  /// <param name="disposing">A value indicating whether disposal of managed resources should be performed.</param>
  protected override void Dispose(bool disposing)
  {
    _subscription.Dispose();
    AuthenticationStateChanged -= OnAuthenticationStateChanged;
    base.Dispose(disposing);
  }
}