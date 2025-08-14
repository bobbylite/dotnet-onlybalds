using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a questionnaire item.
/// </summary>
[Table("QuestionnaireItems")]
public class QuestionnaireItems
{
    /// <summary>
    /// Unique identifier for the questionnaire item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The last name of the user.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the user.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user identifier associated with the questionnaire.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;


    /// <summary>
    /// The address of the user.
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the questionnaire.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the questionnaire is completed.
    /// </summary>
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// The data collected from the questionnaire.
    /// </summary>
    [JsonPropertyName("data")]
    public QuestionnaireData? Data { get; set; }

    /// <summary>
    /// Gets or sets the account associated with the questionnaire.
    /// </summary>
    public Guid AccountId { get; set; }
}

/// <summary>
/// Represents the data collected from the questionnaire.
/// </summary>
public class QuestionnaireData
{
    /// <summary>
    /// Unique identifier for the questionnaire item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the questionnaire identifier.
    /// </summary>
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// The type of baldness the user identifies with.
    /// </summary>
    [JsonPropertyName("baldType")]
    public string BaldType { get; set; } = string.Empty;

    /// <summary>
    /// The methods the user employs to clean their bald head.
    /// </summary>
    [JsonPropertyName("cleaningMethods")]
    public List<string> CleaningMethods { get; set; } = new List<string>();

    /// <summary>
    /// Additional cleaning methods the user employs.
    /// </summary>
    [JsonPropertyName("cleaningMethodsOther")]
    public string? CleaningMethodsOther { get; set; }

    /// <summary>
    /// The user's bald care routine.
    /// </summary>
    [JsonPropertyName("baldCareRoutine")]
    public string? BaldCareRoutine { get; set; }

    /// <summary>
    /// The products the user employs for bald care.
    /// </summary>
    [JsonPropertyName("productsUsed")]
    public string? ProductsUsed { get; set; }

    /// <summary>
    /// The user's monthly spending on bald care products.
    /// </summary>
    [JsonPropertyName("monthlySpend")]
    public string? MonthlySpend { get; set; }

    /// <summary>
    /// The user's confidence level regarding their baldness.
    /// </summary>
    [JsonPropertyName("confidenceLevel")]
    public string? ConfidenceLevel { get; set; }

    /// <summary>
    /// The user's interests related to baldness.
    /// </summary>
    [JsonPropertyName("interests")]
    public List<string> Interests { get; set; } = new List<string>();

    /// <summary>
    /// The user's goals related to baldness.
    /// </summary>
    [JsonPropertyName("goals")]
    public List<string> Goals { get; set; } = new List<string>();

    /// <summary>
    /// Additional goals the user has related to baldness.
    /// </summary>
    [JsonPropertyName("goalsOther")]
    public string? GoalsOther { get; set; }

    /// <summary>
    /// The user's new bald care routine.
    /// </summary>
    [JsonPropertyName("newRoutine")]
    public string? NewRoutine { get; set; }
}
