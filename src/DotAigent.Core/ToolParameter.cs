namespace DotAigent.Core;

public record ToolParameterDescription(string Name, string Description, string Type, bool Required);
public record ToolParameter(string Name, string Value);
