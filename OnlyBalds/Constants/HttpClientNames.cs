namespace OnlyBalds;

/// <summary>
/// Constants representing the names of HTTP clients.
/// </summary>
public static class HttpClientNames
{
    /// <summary>
    /// The name of the HTTP client used to communicate with the Authorization API.
    /// </summary>
    public const string OnlyBaldsAuthenticationToken = "OnlyBalds.Api.Authentication.Token";

    /// <summary>
    /// The name of the HTTP client used to communicate with the OnlyBalds API.
    /// </summary>
    public const string OnlyBalds = "OnlyBalds.Api";

    /// <summary>
    /// The name of the HTTP client used to communicate with the HuggingFace Inference API.
    /// </summary>
    public const string HuggingFaceInferenceApi = "HuggingFace.Inference.Api";

    /// <summary>
    /// The name of the HTTP client used to communicate with the Private HuggingFace Inference API.
    /// </summary>
    public const string PrivateHuggingFaceInferenceApi = "Private.HuggingFace.Inference.Api";
}
