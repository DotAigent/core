
using DotAigent.Core.Agents;

namespace DotAigent.Core;

/// <summary>
/// Builder for creating agent instances
/// </summary>
public class AgentBuilder : IAgentBuilder
{
    private IModel? _model;
    private readonly List<ITool> _tools = new();
    private string _defaultPrompt = "";

    /// <summary>
    /// Set the AI model to use
    /// </summary>
    public IAgentBuilder WithModel(IModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        return this;
    }

    /// <summary>
    /// Add a single tool
    /// </summary>
    public IAgentBuilder WithTool(ITool tool)
    {
        if (tool != null) _tools.Add(tool);
        return this;
    }

    /// <summary>
    /// Add multiple tools
    /// </summary>
    public IAgentBuilder WithTools(IEnumerable<ITool> tools)
    {
        if (tools != null) _tools.AddRange(tools.Where(t => t != null));
        return this;
    }

    /*/// <summary>*/
    /*/// Set an optional default prompt*/
    /*/// </summary>*/
    /*public IAgentBuilder WithDefaultPrompt(string defaultPrompt)*/
    /*{*/
    /*    _defaultPrompt = defaultPrompt ?? "";*/
    /*    return this;*/
    /*}*/

    /// <summary>
    /// Build the agent (ChatbotAgent in this case)
    /// </summary>
    public IAgent Build()
    {
        if (_model == null)
            throw new InvalidOperationException("An AI model must be specified before building the agent.");

        return new ChatbotAgent(_model, _tools, _defaultPrompt);
    }
}
