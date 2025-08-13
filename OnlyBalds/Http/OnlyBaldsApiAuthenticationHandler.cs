using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace OnlyBalds.Http;

public class OnlyBaldsApiAuthenticationHandler : DelegatingHandler
{
    private readonly ILogger<OnlyBaldsApiAuthenticationHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OnlyBaldsApiAuthenticationHandler(
        ILogger<OnlyBaldsApiAuthenticationHandler> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext 
                          ?? throw new InvalidOperationException("No active HttpContext.");

        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogWarning("Access token missing or expired. Triggering OIDC challenge...");
            
            var authProps = new AuthenticationProperties { RedirectUri = "/" };
            await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authProps);

            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        _logger.LogDebug("Using access token for API call");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("API returned 401. Triggering OIDC challenge to refresh token...");
            var authProps = new AuthenticationProperties { RedirectUri = "/" };
            await httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, authProps);
        }

        return response;
    }
}
