using System.Net;
using System.Net.Http.Headers;
using Ardalis.GuardClauses;
using OnlyBalds.Services.Token;
using Microsoft.Extensions.Logging;

namespace OnlyBalds.Http;

/// <summary>
/// An HTTP handler for processing PingIdentity requests and responses in order to add authorization capabilities.
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly ITokenService _authenticateService;

    /// <summary>
    /// Creates an instance of AuthenticationHandler.
    /// </summary>
    /// <param name="logger">The ILoggerAdapter implementation used for logging messages.</param>
    /// <param name="tokenService">Represents a service responsible for performing authenticate operations with PingIdentity.</param>
    /// <exception cref="ArgumentNullException">Thrown if any arguments are null.</exception>
    public AuthenticationHandler(
        ILogger<AuthenticationHandler> logger,
        ITokenService tokenService)
    {
        _logger = Guard.Against.Null(logger);
        _authenticateService = Guard.Against.Null(tokenService);
    }
        
    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending an HTTP request");
        
        HttpResponseMessage response = await PerformRequest(request, cancellationToken);
        
        _logger.LogDebug("The status code for the HTTP request response is '{StatusCode}'", response.StatusCode);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            _logger.LogInformation("The HTTP response indicates an invalid session ID, performing authorize operation");
            await _authenticateService.AuthenticateAsync();
            _logger.LogInformation("Retrying the HTTP request after performing the authorize operation");
            response = await PerformRequest(request, cancellationToken);
        }
        
        _logger.LogTrace("Completed sending an HTTP request");

        return response;
    }

    private async Task<HttpResponseMessage> PerformRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Performing an HTTP request");
        
        _logger.LogDebug("Setting the 'Authorization' header prior to performing the HTTP request");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticateService.Token);

        _logger.LogTrace("Continuing the HTTP request");
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        
        _logger.LogTrace("Completed performing an HTTP request");
        
        return response;
    }
}
