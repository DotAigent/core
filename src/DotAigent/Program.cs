using DotAigent.Core;
using DotAigent.Models;
using DotNetEnv;

Env.Load();

var agent = new AgentBuilder()
    .UsingOpenAiApi()
        .WithUri(new Uri("http://localhost:11434/v1")) // Local ollama
        .WithModelName("qwen2.5:14b")
        .WithTool(new CalcAddTool())
        /*.WithModelName("gemma3:12b")*/
    .Build();

var response = await agent.ProcessInputAsync("Add the numbers 15 and 16");
Console.WriteLine($"Agent: {response}\n");

public class CalcAddTool : ITool
{
    public string Name => "Add";

    public IEnumerable<ToolParameterDescription> Parameters => 
        [
            new ToolParameterDescription("A", "First number to add.", "number", true),
            new ToolParameterDescription("B", "Second number to add.", "number", true),
        ];

    public string Description => "Calculates the sum of two numbers.";

    public Task<string> ExecuteAsync(IEnumerable<ToolParameter> toolParameter)
    {
        var paramA = toolParameter.First(n => n.Name == "A").Value;
        var paramB = toolParameter.First(n => n.Name == "B").Value;

        if (!double.TryParse(paramA, out double a))
                throw new ArgumentException("A must be a number.");
        if (!double.TryParse(paramB, out double b))
                throw new ArgumentException("A must be a number.");
        var sum = a + b;

        return Task.FromResult(sum.ToString());
    }
}

