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
    Task<string> GenerateResponseAsync(string prompt);

    /// <summary>
    /// Set the models system prompt
    /// </summary>
    /// <param name="systemPrompt">The system prompt to set for the model</param>
    /// <returns>A task that resolves to the generated text response from the model.</returns>
    /// <remarks>
    /// An empty string will use a default one for the model.
    /// </remarks>
    void SetSystemPrompt(string systemPrompt);
}
