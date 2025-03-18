namespace DotAigent.Core;

public interface IProvider
{
    string ModelName { get; }
    Uri? Endpoint { get; }

    Task<IAgentResponse<T>> GenerateResponseAsync<T>(string prompt, string? systemPrompt = null, IEnumerable<ITool>? tools = null) where T:class;
}

