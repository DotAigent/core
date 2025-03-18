namespace DotAigent.Core;

/// <summary>
/// Implementation of the IAgent interface providing chatbot functionality.
/// </summary>
/// <param name="provider">The provider used to generate AI responses.</param>
/// <param name="tools">Optional collection of tools available to the agent.</param>
/// <param name="systemPrompt">Optional system instructions to guide the AI model's behavior.</param>
internal class ChatbotAgent(IProvider provider, IEnumerable<ITool>? tools, string? systemPrompt) : IAgent
{
    private readonly IProvider _provider = provider;
    private readonly IEnumerable<ITool>? _tools = tools;
    private readonly string? _systemPrompt = systemPrompt;

    /// <inheritdoc />
    public IEnumerable<ITool> Tools => _tools ?? [];

    /// <inheritdoc />
    public Task<IAgentResponse<T>> GenerateResponseAsync<T>(string prompt) where T:class
    {
        return _provider.GenerateResponseAsync<T>(prompt, _systemPrompt, _tools);
    }
}
