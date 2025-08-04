using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using OnlyBalds.Services.Token;

namespace OnlyBalds.Http;

/// <summary>
/// Represents a handler responsible for authenticating HTTP requests.
/// </summary>
/// <remarks>
/// This class is used to authenticate HTTP requests.
/// </remarks>
/// <seealso cref="DelegatingHandler" />
public class OnlyBaldsApiAuthenticationHandler : DelegatingHandler
{
    private readonly ILogger<OnlyBaldsApiAuthenticationHandler> _logger;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlyBaldsApiAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="tokenService">The token service.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="OnlyBaldsApiAuthenticationHandler"/> class.
    /// </remarks>
    public OnlyBaldsApiAuthenticationHandler(
        ILogger<OnlyBaldsApiAuthenticationHandler> logger,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method sends an HTTP request.
    /// </remarks>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending HTTP request");

        var httpContext = _httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogInformation("No valid token found, requesting a new token");
            await _tokenService.AuthenticateAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.Token);
            return await base.SendAsync(request, cancellationToken);
        }

        _logger.LogDebug("Using existing access token for request");
        _logger.LogDebug("The access token is '{AccessToken}'", accessToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}