
namespace DotAigent.Core.Agents;

public class ChatbotAgent : AgentBase
{
    public ChatbotAgent(IModel model, IEnumerable<ITool> tools, string defaultPrompt = "")
        : base(model, tools, defaultPrompt) { }

    public override async Task<string> ProcessInputAsync(string input)
    {
        try
        {
            if (input.Contains("current events", StringComparison.OrdinalIgnoreCase))
            {
                var searchTool = GetToolByName("Search");
                if (searchTool == null) return "No search tool available.";
                var parameters = new Dictionary<string, object> { { "query", input } };
                var result = await searchTool.ExecuteAsync(parameters);
                return result.ToString() ?? "No results found.";
            }
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



