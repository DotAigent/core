using DotAigent.Core;

namespace DotAigent.Core;

public record AgentResponse<T> : IAgentResponse<T>
{
    public bool Success { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public IEnumerable<ChatMessage> Messages { get; init; } = [];
    public T? Result { get; init; }
}
