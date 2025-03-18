namespace DotAigent.Core;

/// <summary>
/// Represents a provider for AI model services that can generate responses based on prompts.
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Gets the name of the AI model used by this provider.
    /// </summary>
    /// <value>The string identifier of the model.</value>
    string ModelName { get; }
    
    /// <summary>
    /// Gets the endpoint URI for the AI service.
    /// </summary>
    /// <value>The URI of the service endpoint, or null if using default endpoints.</value>
    Uri? Endpoint { get; }

    /// <summary>
    /// Generates a response asynchronously based on the provided prompt.
    /// </summary>
    /// <typeparam name="T">The type of result to be returned by the agent.</typeparam>
    /// <param name="prompt">The user's prompt to send to the AI model.</param>
    /// <param name="systemPrompt">Optional system instructions to guide the AI model's behavior.</param>
    /// <param name="tools">Optional collection of tools that the AI can use when generating a response.</param>
    /// <returns>A task that resolves to an agent response containing the generated content.</returns>
    Task<IAgentResponse<T>> GenerateResponseAsync<T>(string prompt, string? systemPrompt = null, IEnumerable<ITool>? tools = null) where T:class;
}

