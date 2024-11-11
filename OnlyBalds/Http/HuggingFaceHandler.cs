using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using OnlyBalds.Configuration;

namespace OnlyBalds.Http;

/// <summary>
/// Represents a handler responsible for authenticating HTTP requests.
/// </summary>
public class HuggingFaceHandler : DelegatingHandler
{
    private readonly ILogger<AuthenticationHandler> _logger;
    private readonly InferenceApiOptions _inferenceApiOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceHandler"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="inferenceApiOptions"></param>
    public HuggingFaceHandler(
        ILogger<AuthenticationHandler> logger,
        IOptionsSnapshot<InferenceApiOptions> inferenceApiOptions)
    {
        ArgumentNullException.ThrowIfNull(inferenceApiOptions?.Value);
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _inferenceApiOptions = inferenceApiOptions.Value;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending HTTP request after performing authorization");
        var response = await PerformRequest(request, cancellationToken);
        _logger.LogDebug("Completed sending the request with status code '{StatusCode}'", response.StatusCode);

        return response;
    }

    private async Task<HttpResponseMessage> PerformRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {        
        _logger.LogDebug("Setting the 'Authorization' header before performing the request");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _inferenceApiOptions.ApiKey);

        _logger.LogDebug("Continuing the request");
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        
        _logger.LogDebug("Completed performing the request");
        
        return response;
    }
}