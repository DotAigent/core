using OpenAI.Chat;

namespace DotAigent.Providers.OpenAi;

public class ChatMessageList : List<ChatMessage>
{
    public void AddSystemMessage(string systemMessage)
    {
        Add(new SystemChatMessage(systemMessage));
    }

    public void AddUserMessage(string userMessage)
    {
        Add(new UserChatMessage(userMessage));
    }
}
