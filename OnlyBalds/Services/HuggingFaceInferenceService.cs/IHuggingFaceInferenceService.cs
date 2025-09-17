using OnlyBalds.Models;

namespace OnlyBalds.Services;

/// <summary>
/// Represents a service for interacting with the Hugging Face API.
/// </summary>
public interface IHuggingFaceInferenceService
{
    /// <summary>
    /// Analyzes the toxicity of the specified text using the Hugging Face model: 
    /// https://huggingface.co/s-nlp/roberta_toxicity_classifier
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>A collection of toxicity scores.</returns>
    Task<IEnumerable<InferenceModel>> UseRobertaToxicityClassifier(string text);
}