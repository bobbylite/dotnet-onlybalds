using System.Net;
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
/// <seealso cref="AuthenticationHandler" />
public class AuthenticationHandler : DelegatingHandler
{
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly ITokenService _authenticateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="tokenService">The token service.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </remarks>
    /// <seealso cref="AuthenticationHandler" />
    public AuthenticationHandler(
        ILogger<AuthenticationHandler> logger,
        ITokenService tokenService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(tokenService);

        _logger = logger;
        _authenticateService = tokenService;
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
        
        HttpResponseMessage response = await PerformRequest(request, cancellationToken);
        
        _logger.LogDebug("The status code for the request response is '{StatusCode}'", response.StatusCode);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            _logger.LogInformation("The HTTP response contains an invalid token, performing authorization");
            await _authenticateService.AuthenticateAsync();
            _logger.LogInformation("Retrying the request after performing authorization");
            response = await PerformRequest(request, cancellationToken);
        }
        
        _logger.LogTrace("Completed sending the request");

        return response;
    }

    private async Task<HttpResponseMessage> PerformRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Performing HTTP request");
        
        _logger.LogDebug("Setting the 'Authorization' header before performing the request");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticateService.Token);

        _logger.LogDebug("Continuing the request");
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        
        _logger.LogDebug("Completed performing the request");
        
        return response;
    }
}
