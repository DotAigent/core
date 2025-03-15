
namespace DotAigent.Core.Agents;

public class ChatbotAgent : AgentBase
{
    public ChatbotAgent(IModel model, IEnumerable<ITool> tools, string defaultPrompt = "")
        : base(model, tools, defaultPrompt) { }

    public override async Task<string> ProcessInputAsync(string input)
    {
        try
        {
            var prompt = string.IsNullOrEmpty(DefaultPrompt) ? input : $"{DefaultPrompt}: {input}";
            return await Model.GenerateResponseAsync(prompt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ChatbotAgent: {ex.Message}");
            return "Sorry, an error occurred.";
        }
    }
}



