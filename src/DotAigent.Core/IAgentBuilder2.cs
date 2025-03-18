using System.Text.Json;
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

public interface IAgentBuilder2 : IAgentSupport, IProviderSupport
{

}


public interface IProviderBuilder: IBuildSupport
{
}

public interface IProviderSupport
{
    IProviderBuilder UsingProvider(IProvider provider);
}

public class AgentBuilder2 : IAgentBuilder2,  IProviderBuilder
{
    private IProvider? _provider;
    private string _systemPrompt = string.Empty;
    private string _modelName = string.Empty;
    private Type? _resultType;
    private Uri? _endpoint;
    private readonly List<ITool> _tools =[];

    public IAgent Build()
    {


        var agent = new ChatbotAgent2(
                _provider ?? throw new InvalidOperationException("Provider is required"),
                _tools, 
                _systemPrompt);

        return agent;
    }

    public IProviderBuilder WithEndpoint(Uri uri)
    {
        _endpoint = uri;
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

    public IProviderBuilder UsingProvider(IProvider provider)
    {
        _provider = provider;
        return this;
    }
}

