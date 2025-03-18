namespace DotAigent.Core;

public interface ITool
{
    /// <summary>
    /// Gets the name of the tool. This should be unique and descriptive.
    /// </summary>
    string Name { get; } 
    
    /// <summary>
    /// Gets the human-readable description of what the tool does.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the parameters that this tool accepts, including their descriptions and constraints.
    /// </summary>
    IEnumerable<ToolParameterDescription> Parameters { get; }
}

/// <summary>
/// Interface representing a function-based tool that can be used by an AI agent to perform specific tasks.
/// </summary>
public interface IFunctionTool : ITool
{ 
    
    /// <summary>
    /// Executes the tool with the provided parameters.
    /// </summary>
    /// <param name="toolParameter">The collection of parameters to use when executing the tool.</param>
    /// <returns>A string result of the tool execution.</returns>
    Task<string> ExecuteAsync(IEnumerable<ToolParameter> toolParameter);
}

/// <summary>
/// Interface representing a tool that leverages another agent to perform tasks.
/// </summary>
public interface IAgentTool : ITool
{
    // This interface is currently empty but provides a type distinction for agent-based tools
}
