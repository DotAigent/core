namespace DotAigent.Core.Agents;

using DotAigent.Core;

public abstract class AgentBase(IModel model) : IAgent
{
    protected readonly IModel Model = model ?? throw new ArgumentNullException(nameof(model));

    public abstract Task<string> GenerateResponseAsync(string input);
}
