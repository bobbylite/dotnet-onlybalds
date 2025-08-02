using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlyBalds.Client.Models;

/// <summary>
/// Represents a questionnaire item for the balding website.
/// This questionnaire is used to target specific products for users.
/// </summary>
public class QuestionnaireItems
{
    /// <summary>
    /// Gets or sets the unique identifier for the questionnaire item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the questionnaire is completed.
    /// </summary>
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the start date of the questionnaire.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the data associated with the questionnaire.
    /// </summary>
    [JsonPropertyName("data")]
    public QuestionnaireData? Data { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the questionnaire.
    /// /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the identity.
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the user filling out the questionnaire.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the user filling out the questionnaire.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address of the user filling out the questionnaire.
    /// </summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
}

/// <summary>
/// Represents the data associated with a questionnaire.
/// </summary>
public class QuestionnaireData
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the list of questions in the questionnaire.
    /// </summary>
    [JsonPropertyName("questions")]
    public IEnumerable<Question>? Questions { get; set; }

    /// <summary>
    /// Gets or sets the balding options available in the questionnaire.
    /// </summary>
    [JsonPropertyName("baldingOptions")]
    public IEnumerable<BaldingOptions>? BaldingOptions { get; set; }
}

/// <summary>
/// Represents a single question in the questionnaire.
/// </summary>
public class Question
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the question.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the answer to the question.
    /// </summary>
    [JsonPropertyName("answer")]
    public string Answer { get; set; } = string.Empty;
}

/// <summary>
/// Represents the balding options available in the questionnaire.
/// </summary>
public class BaldingOptions
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the list of balding options.
    /// </summary>
    [JsonPropertyName("option")]
    public IEnumerable<BaldingOption>? Option { get; set; }
}

/// <summary>
/// Represents a single balding option in the questionnaire.
/// </summary>
public class BaldingOption
{
    /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the balding option.
    /// </summary>
    [JsonPropertyName("baldingOptionTitle")]
    public string BaldingOptionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of questions for the balding option.
    /// </summary>
    [JsonPropertyName("questions")]
    public IEnumerable<Question>? Questions { get; set; }
}
