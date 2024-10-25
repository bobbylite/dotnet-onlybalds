using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents a questionnaire item for the balding website.
/// This questionnaire is used to target specific products for users.
/// </summary>
[Table("QuestionnaireItems")]
public class QuestionnaireItems
{
    /// <summary>
    /// Gets or sets the unique identifier for the questionnaire item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the questionnaire is completed.
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the start date of the questionnaire.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the data associated with the questionnaire.
    /// </summary>
    public QuestionnaireData? Data { get; set; }
}

/// <summary>
/// Represents the data associated with a questionnaire.
/// </summary>
public class QuestionnaireData
{
        /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the list of questions in the questionnaire.
    /// </summary>
    public IEnumerable<string>? Questions { get; set; }

    /// <summary>
    /// Gets or sets the balding options available in the questionnaire.
    /// </summary>
    public IEnumerable<BaldingOptions>? BaldingOptions { get; set; }
}

/// <summary>
/// Represents the balding options available in the questionnaire.
/// </summary>
public class BaldingOptions
{
     /// <summary>
    /// Gets or sets the unique identifier for the item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the list of balding options.
    /// </summary>
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
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the balding option.
    /// </summary>
    public string BaldingOptionTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of questions for the balding option.
    /// </summary>
    public IEnumerable<string>? Questions { get; set; }
}