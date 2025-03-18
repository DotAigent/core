namespace DotAigent.Core;

/// <summary>
/// The response type from an agent.
/// </summary>
/// <typeparam name="T">The type of result data returned by the agent.</typeparam>
public interface IAgentResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the agent operation was successful.
    /// </summary>
    /// <value><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</value>
    bool Success { get; }

    /// <summary>
    /// Gets the error message if the agent operation failed.
    /// </summary>
    /// <value>A string containing the error details when <see cref="Success"/> is <c>false</c>; otherwise an empty string.</value>
    string ErrorMessage { get; }

    /// <summary>
    /// Gets the collection of chat messages exchanged during the agent interaction.
    /// </summary>
    /// <value>An enumerable collection of <see cref="ChatMessage"/> objects representing the conversation history.</value>
    IEnumerable<ChatMessage> Messages { get; } 

    /// <summary>
    /// Gets the result data returned by the agent.
    /// </summary>
    /// <value>The result object of type <typeparamref name="T"/> if <see cref="Success"/> is <c>true</c>; otherwise <c>null</c>.</value>
    T? Result { get; }
}
