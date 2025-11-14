using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Options;
using OnlyBalds.Configuration;
using OnlyBalds.Models;

namespace OnlyBalds.Services;

/// <summary>
/// Represents a service for interacting with the Hugging Face API.
/// </summary>
public class HuggingFaceInferenceService : IHuggingFaceInferenceService
{
    private readonly ILogger<HuggingFaceInferenceService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly InferenceApiOptions _apiOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceInferenceService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> or 
    /// <paramref name="httpClientFactory"/> is <see langword="null"/>.</exception>
    public HuggingFaceInferenceService(
        ILogger<HuggingFaceInferenceService> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<InferenceApiOptions> apiOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(apiOptions);

        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _apiOptions = apiOptions.CurrentValue;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<InferenceModel>> UseRobertaToxicityClassifier(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var inferenceInputs = new { inputs = text };

        _logger.LogDebug("Creating a new HTTP client for the Hugging Face Inference API");
        var inferenceClient = _httpClientFactory.CreateClient(HttpClientNames.HuggingFaceInferenceApi);

        _logger.LogDebug("Performing toxicity classification using the Hugging Face Inference API");

        var json = JsonSerializer.Serialize(inferenceInputs);

        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var inferenceResponsePayload = await inferenceClient.PostAsync(
            _apiOptions.BaseUrl,
            content
        );

        if (!inferenceResponsePayload.IsSuccessStatusCode)
        {
            _logger.LogError("Status code: {StatusCode}", inferenceResponsePayload.StatusCode);
            return null!;
        }

        _logger.LogDebug("Reading the inference model content asynchronously");
        var inferenceContent = await inferenceResponsePayload.Content.ReadAsStringAsync();

        _logger.LogDebug("Deserializing the inference model");
        _logger.LogInformation(inferenceContent);
        var inferenceModel = JsonSerializer.Deserialize<IEnumerable<IEnumerable<InferenceModel>>>(inferenceContent);

        if (inferenceModel is null)
        {
            _logger.LogError("Failed to deserialize the inference model");
            return null!;
        }

        var inference = inferenceModel.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(inference);

        _logger.LogDebug("Successfully classified text with Roberta toxicity classifier");
        return inference;
    }
}