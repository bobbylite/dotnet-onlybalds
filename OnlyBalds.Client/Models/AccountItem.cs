namespace OnlyBalds.Client.Models;

public class AccountItem
{
    /// <summary>
    /// Unique identifier for the account.
    /// </summary>
    public Guid Id { get; set; }

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
    public bool HasSubmittedQuistionnaire { get; set; }

    /// <summary>
    /// The identifier of the questionnaire.
    /// </summary>
    public string QuestionnaireId { get; set; } = string.Empty;
}
