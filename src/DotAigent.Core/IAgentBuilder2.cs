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
    IProviderBuilder UsingProvider(IProvider provider);
}
public interface IProvider
{
}

public class AgentBuilder2 : IAgentBuilder2, IModelBuilder, IProviderBuilder
{
    private IModel? _model;
    private IProvider? _provider;
    private string _systemPrompt = string.Empty;
    private string _modelName = string.Empty;
    private Type? _resultType;
    private Uri? _endpoint;
    private List<ITool> _tools =[];

    public IAgent Build()
    {
        
        
        var agent = new ChatbotAgent2(_model, _tools, _systemPrompt);
        return agent;
        
    }

    public IProviderBuilder WithEndpoint(Uri uri)
    {
        _endpoint = uri;
        return this;
    }

    public IProviderBuilder WithProvider(IProvider provider)
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

    public IProviderBuilder UsingProvider(IProvider provider)
    {
        _provider = provider;
        return this;
    }
}

internal class ChatbotAgent2 : IAgent
{
    private IModel? model;
    private List<ITool> tools;
    private string systemPrompt;

    public ChatbotAgent2(IModel? model, List<ITool> tools, string systemPrompt)
    {
        this.model = model;
        this.tools = tools;
        this.systemPrompt = systemPrompt;
    }

    public IEnumerable<ITool> Tools => throw new NotImplementedException();

    public Task<AiAgentResponse> GenerateResponseAsync(string prompt)
    {
        throw new NotImplementedException();
    }
}

internal interface IModelBuilder
{
}

public static class Providers
{

    public static IProvider OpenAI => new OpenAIProvider();

}

public class OpenAIProvider : IProvider
{

}
