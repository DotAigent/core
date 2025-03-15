namespace DotAigent.Core;

/// <summary>
/// Describes a parameter that can be used with a tool, including its metadata.
/// </summary>
/// <param name="Name">The name of the parameter.</param>
/// <param name="Description">Human-readable description explaining the parameter's purpose.</param>
/// <param name="Type">The data type of the parameter (e.g., "string", "integer", "boolean").</param>
/// <param name="Required">Indicates whether the parameter must be provided when executing the tool.</param>
public record ToolParameterDescription(string Name, string Description, string Type, bool Required);

/// <summary>
/// Represents a parameter value passed to a tool during execution.
/// </summary>
/// <param name="Name">The name of the parameter, which should match a corresponding ToolParameterDescription.</param>
/// <param name="Value">The string representation of the parameter value to be used by the tool.</param>
public record ToolParameter(string Name, string Value);
