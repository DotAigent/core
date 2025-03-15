namespace DotAigent.Core;

/// <summary>
/// Represents an AI agent that can process user inputs and generate responses.
/// This interface serves as the primary abstraction for agent implementations
/// that handle interaction between users and the underlying AI models.
/// </summary>
public interface IAgent
{
    /// <summary>
    /// Processes user input asynchronously and generates a response.
    /// </summary>
    /// <param name="prompt">The user's input text to be processed by the agent.</param>
    /// <returns>
    /// A task that resolves to the agent's response as a string.
    /// This typically includes the AI model's generated content, potentially
    /// enhanced by tool executions or other agent capabilities.
    /// </returns>
    Task<string> GenerateResponseAsync(string prompt);
}
