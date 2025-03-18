namespace DotAigent.Core;

/// <summary>
/// Represents a message in a chat conversation.
/// </summary>
public record ChatMessage
{
    /// <summary>
    /// Gets the role of the entity that sent this message.
    /// </summary>
    /// <value>The <see cref="ChatRole"/> indicating whether the message is from the system, user, agent, or tool.</value>
    public ChatRole Role { get; init; }
    
    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    /// <value>The text content of the message. Empty string by default.</value>
    public string Message { get; init; } = string.Empty;
}

