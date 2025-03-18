namespace DotAigent.Core;

public record AIModelParameters
{
    public Uri? ApiEndpoint { get; init; }
    public string? ModelName { get; init; } = string.Empty;
    public string? ApiKey { get; init; } = string.Empty;
}

public record AiAgentResponse
{
    public bool Success { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public IEnumerable<AiChatMessage> Messages { get; init; } = [];
    public AiChatMessage? Result { get; init; }
}

public interface IAgentResponse<T>
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<AiChatMessage> Messages { get; } 
    T? Result { get; }
}


public record AiChatMessage
{
    public ChatRole Role { get; init; }
    public string Message { get; init; } = string.Empty;
}

public enum ChatRole
{
    System,
    User,
    Agent,
    Tool
}
