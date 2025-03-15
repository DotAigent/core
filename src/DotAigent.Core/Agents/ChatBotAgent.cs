
namespace DotAigent.Core.Agents;

public class ChatbotAgent(IModel model) : AgentBase(model)
{
    public override async Task<string> GenerateResponseAsync(string prompt)
    {
        try
        {
            return await Model.GenerateResponseAsync(prompt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ChatbotAgent: {ex.Message}");
            return "Sorry, an error occurred.";
        }
    }
}



