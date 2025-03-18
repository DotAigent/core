namespace DotAigent.Core;

internal class ChatbotAgent2(IProvider provider, IEnumerable<ITool> tools, string systemPrompt) : IAgent
{
    private readonly IProvider _provider = provider;
    private readonly IEnumerable<ITool> _tools = tools;
    private readonly string _systemPrompt = systemPrompt;

    public IEnumerable<ITool> Tools => _tools;

    public Task<IAgentResponse<T>> GenerateResponseAsync<T>(string prompt) where T:class
    {
        return _provider.GenerateResponseAsync<T>(prompt, _systemPrompt, _tools);
    }
}
