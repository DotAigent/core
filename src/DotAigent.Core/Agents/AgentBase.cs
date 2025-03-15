namespace DotAigent.Core.Agents;

using DotAigent.Core;

public abstract class AgentBase : IAgent
{
    protected readonly IModel Model;
    protected readonly IList<ITool> Tools;
    protected readonly string DefaultPrompt;

    protected AgentBase(IModel model, IEnumerable<ITool> tools, string defaultPrompt = "")
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Tools = tools?.ToList() ?? [];
        DefaultPrompt = defaultPrompt;
    }

    public abstract Task<string> ProcessInputAsync(string input);

    protected ITool? GetToolByName(string name)
    {
        return Tools.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
