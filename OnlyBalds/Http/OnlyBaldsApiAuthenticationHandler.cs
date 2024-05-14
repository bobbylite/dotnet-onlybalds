using System.Net.Http.Headers;
using OnlyBalds.Services.Token;

namespace OnlyBalds.Http;

/// <summary>
/// Represents a type that is used to handle API authentication by delegating the HTTP requests.
/// </summary>
public class OnlyBaldsApiAuthenticationHandler : DelegatingHandler
{
    private ILogger<OnlyBaldsApiAuthenticationHandler> _logger;
    private ITokenService _tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlyBaldsApiAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger to be used for logging information.</param>
    /// <param name="tokenService">The service to be used for token operations.</param>
    public OnlyBaldsApiAuthenticationHandler(
        ILogger<OnlyBaldsApiAuthenticationHandler> logger,
        ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation, adding authentication header if necessary.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message that the server sends back.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tokenService.Token))
        {
            _logger.LogInformation("No valid token found, requesting a new token");
            await _tokenService.AuthenticateAsync();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}