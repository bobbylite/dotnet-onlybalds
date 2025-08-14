using System.Text.Json.Serialization;

namespace OnlyBalds.Models;

public class AccountItem
{
    /// <summary>
    /// Unique identifier for the account.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the user filling out the questionnaire.
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

    /// <summary>
    /// The identity provider identifier.
    /// </summary>
    public string IdentityProviderId { get; set; } = string.Empty;

    /// <summary>
    /// The username of the account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email of the account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Represents whether the identity of the account has submitted the questionnaire.
    /// </summary>
    public bool HasSubmittedQuestionnaire { get; set; }

    /// <summary>
    /// The identifier of the questionnaire.
    /// </summary>
    public string QuestionnaireId { get; set; } = string.Empty;
}
