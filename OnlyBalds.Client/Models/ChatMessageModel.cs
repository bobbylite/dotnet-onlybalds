using System;

namespace OnlyBalds.Client.Models;

/// <summary>
/// Represents a chat message model.
/// </summary>
public class ChatMessageModel
{
    /// <summary>
    /// Gets or sets the user who sent the message.
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
