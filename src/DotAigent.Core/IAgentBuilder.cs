using System.Text.Json;
using DotAigent.Core.Agents;

namespace DotAigent.Core;

public interface IAgentSupport
{
    /// <summary>
    /// Set the system prompt
    /// </summary>
    IAgentBuilder WithSystemPrompt(string systemPrompt);

    /// <summary>
    /// Agent use the provided tool
    /// </summary>
    IAgentBuilder UsingTool(ITool tool);
}

public interface IBuildSupport 
{
    /// <summary>
    /// Build the agent
    /// </summary>
    IAgent Build();
}

public interface IAgentBuilder : IAgentSupport, IProviderSupport {}

public interface IProviderBuilder: IBuildSupport {}

public interface IProviderSupport
{
     
    /// <summary>
    /// Use the specified provider, e.g. OpenAI, Google, Antrophic, etc.
    /// </summary>
    IProviderBuilder UsingProvider(IProvider provider);
}

/// <summary>
/// Agent builder
/// </summary>
public class AgentBuilder : IAgentBuilder,  IProviderBuilder
{
    private IProvider? _provider;
    private string? _systemPrompt;
    private readonly List<ITool> _tools =[];

    /// <inheritdoc />
    public IAgent Build()
    {
        var agent = new ChatbotAgent(
                _provider ?? throw new InvalidOperationException("Provider is required"),
                _tools, 
                _systemPrompt);

        return agent;
    }

    /// <inheritdoc />
    public IAgentBuilder WithSystemPrompt(string systemPrompt)
    {
        _systemPrompt = systemPrompt;
        return this;
    }

    /// <inheritdoc />
    public IAgentBuilder UsingTool(ITool tool)
    {
        _tools.Add(tool);
        return this;
    }

    /// <inheritdoc />
    public IProviderBuilder UsingProvider(IProvider provider)
    {
        _provider = provider;
        return this;
    }
}

