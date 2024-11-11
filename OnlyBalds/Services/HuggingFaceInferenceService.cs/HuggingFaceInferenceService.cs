using System.Text.Json;
using OnlyBalds.Models;

namespace OnlyBalds.Services;

/// <summary>
/// Represents a service for interacting with the Hugging Face API.
/// </summary>
public class HuggingFaceInferenceService : IHuggingFaceInferenceService
{
    private readonly ILogger<HuggingFaceInferenceService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceInferenceService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> or 
    /// <paramref name="httpClientFactory"/> is <see langword="null"/>.</exception>
    public HuggingFaceInferenceService(
        ILogger<HuggingFaceInferenceService> logger,
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<IEnumerable<InferenceModel>>> UseRobertaToxicityClassifier(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var inferenceInputs = new { inputs = text};

        _logger.LogDebug("Creating a new HTTP client for the Hugging Face Inference API");
        var inferenceClient = _httpClientFactory.CreateClient(HttpClientNames.HuggingFaceInferenceApi);

        _logger.LogDebug("Performing toxicity classification using the Hugging Face Inference API");
        var inferenceResponse = await inferenceClient.PostAsJsonAsync("/models/s-nlp/roberta_toxicity_classifier", inferenceInputs);

        if (!inferenceResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Status code: {StatusCode}", inferenceResponse.StatusCode);
            return null!;
        }

        _logger.LogDebug("Reading the inference model content asynchronously");
        var inferenceContent = await inferenceResponse.Content.ReadAsStringAsync();

        _logger.LogDebug("Deserializing the inference model");
        var inferenceModel = JsonSerializer.Deserialize<IEnumerable<IEnumerable<InferenceModel>>>(inferenceContent);

        if (inferenceModel is null)
        {
            _logger.LogError("Failed to deserialize the inference model");
            return null!;
        }

        _logger.LogDebug("Successfully classified text with Roberta toxicity classifier");
        return inferenceModel;
    }
}