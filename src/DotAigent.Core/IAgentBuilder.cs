namespace DotAigent.Core;

/// <summary>
/// Interface for the agent builder pattern
/// </summary>
public interface IAgentBuilder
{
    /// <summary>
    /// Set the system prompt
    /// </summary>
    IAgentBuilder WithSystemPrompt(string systemPrompt);

    /// <summary>
    /// Build the agent
    /// </summary>
    IAgent Build();
}

public interface IToolAgentBuilder : IAgentBuilder
{
    IToolAgentBuilder WithJsonOutputFormat(string jsonOutputFormat);

    /// <summary>
    /// Add a single tool
    /// </summary>
    IToolAgentBuilder WithTool(ITool tool);
    
    /// <summary>
    /// Add multiple tools
    /// </summary>
    IToolAgentBuilder WithTools(IEnumerable<ITool> tools);

    IToolAgentBuilder WithToolAgent(IAgent additionTookAgent);

}


