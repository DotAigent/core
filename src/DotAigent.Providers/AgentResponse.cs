using DotAigent.Core;

namespace DotAigent.Models;

internal record AgentResponse<T> : IAgentResponse<T>
{
    public bool Success { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public IEnumerable<AiChatMessage> Messages { get; init; } = [];
    public T? Result { get; init; }
}
