using System.Text.Json.Serialization;

namespace OnlyBalds.Models;

/// <summary>
/// Represents the result of a text classification prediction from the Hugging Face model.
/// </summary>
public class InferenceModel
{
    /// <summary>
    /// Gets or sets the classification label returned by the Hugging Face model.
    /// This indicates the type of prediction (e.g., "toxic", "obscene", "insult", "identity_hate").
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the confidence score for the associated label.
    /// This is a floating-point value between 0 and 1 that represents the likelihood that the text matches the label.
    /// A higher score indicates a higher likelihood of the label being correct.
    /// </summary>
    [JsonPropertyName("score")]
    public float Score { get; set; }
}