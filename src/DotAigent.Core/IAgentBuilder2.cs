using DotAigent.Core.Agents;

namespace DotAigent.Core;

public interface IAgentSupport
{
    /// <summary>
    /// Set the system prompt
    /// </summary>
    IAgentBuilder2 WithSystemPrompt(string systemPrompt);
    IAgentBuilder2 WithResultType<T>();
    IAgentBuilder2 UsingTool(ITool tool);
}
public interface IBuildSupport 
{
    /// <summary>
    /// Build the agent
    /// </summary>
    IAgent Build();
}

public interface IAgentBuilder2 : IAgentSupport, IProviderSupport, IBuildSupport
{

}


public interface IProviderBuilder: IBuildSupport
{
    IProviderBuilder WithEndpoint(Uri uri);
    IProviderBuilder WithModel(string modelName);
}
public interface IProviderSupport
{
    IProviderBuilder UsingProvider(Provider provider);
}

public class AgentBuilder2 : IAgentBuilder2,  IProviderBuilder
{
    private Provider _provider;
    private string _systemPrompt = string.Empty;
    private string _modelName = string.Empty;
    private Type? _resultType;
    private Uri? _endpoint;
    private readonly List<ITool> _tools =[];

    public IAgent Build()
    {
        var provider = _provider switch
        {
            Provider.OpenAI => new OpenAIProvider(_modelName, _endpoint),
            _ => throw new NotImplementedException()
        };

        var agent = new ChatbotAgent2(
                provider, 
                _tools, 
                _systemPrompt);

        return agent;
    }

    public IProviderBuilder WithEndpoint(Uri uri)
    {
        _endpoint = uri;
        return this;
    }

    public IProviderBuilder WithProvider(Provider provider)
    {
        _provider = provider;
        return this;
    }

    public IAgentBuilder2 WithResultType<T>()
    {
        _resultType = typeof(T);
        return this;
    }

    public IAgentBuilder2 WithSystemPrompt(string systemPrompt)
    {
        _systemPrompt = systemPrompt;
        return this;
    }

    public IAgentBuilder2 UsingTool(ITool tool)
    {
        _tools.Add(tool);
        return this;
    }

    IProviderBuilder IProviderBuilder.WithModel(string modelName)
    {
        _modelName = modelName;
        return this;
    }

    public IProviderBuilder UsingProvider(Provider provider)
    {
        _provider = provider;
        return this;
    }
}

internal class ChatbotAgent2 : IAgent
{
    private readonly IProvider _provider;
    private readonly IEnumerable<ITool> _tools;
    private readonly string _systemPrompt;

    public ChatbotAgent2(IProvider provider, IEnumerable<ITool> tools, string systemPrompt)
    {
        _provider = provider;
        _tools = tools;
        _systemPrompt = systemPrompt;
    }

    public IEnumerable<ITool> Tools => _tools;

    public Task<AiAgentResponse> GenerateResponseAsync(string prompt)
    {
        throw new NotImplementedException();
    }
}


public enum Provider
{
    OpenAI,
}

public class OpenAIProvider(string modelName, Uri? endpoint) : IProvider
{

    public Uri? Endpoint => endpoint;

    string IProvider.ModelName => modelName;
}
