using System.Net.Http.Headers;
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
    private ILogger<OnlyBaldsApiAuthenticationHandler> _logger;
    private ITokenService _tokenService;

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
        ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
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
        if (string.IsNullOrEmpty(_tokenService.Token))
        {
            _logger.LogInformation("No valid token found, requesting a new token");
            await _tokenService.AuthenticateAsync();
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}