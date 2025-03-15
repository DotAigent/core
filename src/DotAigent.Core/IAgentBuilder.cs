namespace DotAigent.Core;

/// <summary>
/// Interface for the agent builder pattern
/// </summary>
public interface IAgentBuilder
{
    /// <summary>
    /// Set the AI model to use
    /// </summary>
    IAgentBuilder WithModel(IModel model);


    /// <summary>
    /// Add a single tool
    /// </summary>
    IAgentBuilder WithTool(ITool tool);
    
    /// <summary>
    /// Add multiple tools
    /// </summary>
    IAgentBuilder WithTools(IEnumerable<ITool> tools);
    
    /// <summary>
    /// Set an optional default prompt
    /// </summary>
    /*IAgentBuilder WithDefaultPrompt(string defaultPrompt);*/
    
    /// <summary>
    /// Build the agent
    /// </summary>
    IAgent Build();
}

