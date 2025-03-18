
using DotAigent.Core;
using DotAigent.Models;

// public class AdditionToolAgent : IFunctionTool
// {
//     public string Name => "calculator_tool";
//
//     public string Description => "use this tool for calcluations like addition, subtraction, multiplication, division";
//
//     public IEnumerable<ToolParameterDescription> Parameters =>
//         [
//             new ToolParameterDescription("query", "Users query for calculation of the sum of two numbers", "string", true),
//         ];
//
//     public async Task<string> ExecuteAsync(IEnumerable<ToolParameter> toolParameters)
//     {
//         var agent = new AgentBuilder()
//         .UsingOpenAiApi()
//             /*.WithModelName("gpt-4o-mini")*/
//             .WithModelName("qwen2.5:14b")
//             .WithUri(new Uri("http://localhost:11434/v1")) // Local ollama
//             .WithTool(new AdditionCalculatorTool())
//         .Build();
//
//         /*Console.WriteLine($"Calculating the sum of two numbers using query {toolParameters}");*/
//         var query = toolParameters.FirstOrDefault(p => p.Name == "query")?.Value ?? throw new ArgumentException("Query must be provided");
//         /*Console.WriteLine($"Calculating the sum of two numbers using query {query}");*/
//         var result = await agent.GenerateResponseAsync(query);
//
//         /*Console.WriteLine($"TOOL: Calculating the sum of two numbers using query {query} equals {result.Result?.Message}");*/
//         if (!result.Success)
//             throw new InvalidOperationException($"Error in calculation, {result.ErrorMessage}");
//         return result.Result?.Message ?? throw new InvalidOperationException("Error in calculation");
//
//     }
// }
//
// public class AdditionCalculatorTool : IFunctionTool
// {
//     public string Name => "Add";
//
//     public IEnumerable<ToolParameterDescription> Parameters =>
//         [
//             new ToolParameterDescription("A", "First number to add.", "integer", true),
//             new ToolParameterDescription("B", "Second number to add.", "integer", true),
//         ];
//
//     public string Description => "Calculates the sum of two numbers.";
//
//     public Task<string> ExecuteAsync(IEnumerable<ToolParameter> parameters)
//     {
//         var firstNumberStr = parameters.First(p => p.Name == "A").Value;
//         var secondNumberStr = parameters.First(p => p.Name == "B").Value;
//
//         if (!double.TryParse(firstNumberStr, out double firstNumber))
//             throw new ArgumentException("A must be a number.");
//         if (!double.TryParse(secondNumberStr, out double secondNumber))
//             throw new ArgumentException("B must be a number.");
//         var result = firstNumber + secondNumber;
//
//         return Task.FromResult($"{result}");
//     }
// }
