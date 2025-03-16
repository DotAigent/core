namespace DotAigent.Core;

/// <summary>
/// Represents a language model that can generate responses based on text prompts.
/// This interface provides a common abstraction for different AI model implementations.
/// </summary>
public interface IModel
{
    /// <summary>
    /// Generates a text response asynchronously based on the provided prompt.
    /// </summary>
    /// <param name="prompt">The input text prompt to send to the language model.</param>
    /// <returns>A task that resolves to the generated text response from the model.</returns>
    Task<AiAgentResponse> GenerateResponseAsync(string prompt, IEnumerable<ITool>? tools = null, string? systemPrompt = null, string? outputFormat = null);

    void SetModelParameters(AIModelParameters parameters);
}

public record AIModelParameters
{
    public Uri? ApiEndpoint { get; init; }
    public string? ModelName { get; init; } = string.Empty;
    public string? ApiKey { get; init; } = string.Empty;
}

public record AiAgentResponse
{
    public bool Success { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public IEnumerable<AiChatMessage> Messages { get; init; } = [];
    public AiChatMessage? Result { get; init; }
}

public record AiChatMessage
{
    public ChatRole Role { get; init; }
    public string Message { get; init; } = string.Empty;
}

public enum ChatRole
{
    System,
    User,
    Agent,
    Tool
}
