using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyBalds.Api.Models;

/// <summary>
/// Represents an account in the system.
/// </summary>
[Table("Accounts")]
public class Account
{
    /// <summary>
    /// Unique identifier for the account.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the identity.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
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
    /// Represents the questionnaire associated with the account.
    /// </summary>
    public QuestionnaireItems? Questionnaire { get; set; } 
}
