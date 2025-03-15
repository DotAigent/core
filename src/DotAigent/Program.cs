using DotAigent.Core;
using DotAigent.Models;
using DotNetEnv;

Env.Load();

var agent = new AgentBuilder()
    .UsingOpenAiApi()
        .WithUri(new Uri("http://localhost:11434/v1")) // Local ollama
        .WithModelName("qwen2.5:14b")
        .WithTool(new AdditionCalculatorTool())
        .WithJsonOutputFormat("""{ "sum": "value" }""")
        /*.WithSystemPrompt("You are a additon tool assistant")*/
        /*.WithModelName("gemma3:12b")*/
    .Build();

var response = await agent.GenerateResponseAsync("Add the numbers 15 and 16");
Console.WriteLine($"{response}");

public class AdditionCalculatorTool : ITool
{
    public string Name => "Add";

    public IEnumerable<ToolParameterDescription> Parameters => 
        [
            new ToolParameterDescription("A", "First number to add.", "number", true),
            new ToolParameterDescription("B", "Second number to add.", "number", true),
        ];

    public string Description => "Calculates the sum of two numbers.";

    public Task<string> ExecuteAsync(IEnumerable<ToolParameter> parameters)
    {
        var firstNumberStr = parameters.First(p => p.Name == "A").Value;
        var secondNumberStr = parameters.First(p => p.Name == "B").Value;

        if (!double.TryParse(firstNumberStr, out double firstNumber))
                throw new ArgumentException("A must be a number.");
        if (!double.TryParse(secondNumberStr, out double secondNumber))
                throw new ArgumentException("B must be a number.");
        var result = firstNumber + secondNumber;

        return Task.FromResult(result.ToString());
    }
}

