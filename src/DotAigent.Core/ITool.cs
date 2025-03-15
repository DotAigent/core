namespace DotAigent.Core;
public interface ITool 
{ 
    string Name { get; } 
    string Description { get; }
    IEnumerable<ToolParameterDescription> Parameters { get; }
    Task<string> ExecuteAsync(IEnumerable<ToolParameter> toolParameter);
}
