
namespace DotAigent.Models;

using DotAigent.Core;

/// <summary>
/// Extension methods for IAgentBuilder
/// </summary>
public static class AgentBuilderExtensions
{
    public static OpenAiAgentBuilder UsingOpenAiApi(this AgentBuilder builder)
    {
        return new OpenAiAgentBuilder(builder);
    }
}

