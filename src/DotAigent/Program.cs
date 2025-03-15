using DotAigent.Core;
using DotAigent.Models;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var agent = new AgentBuilder()
    .UsingOpenAiApi()
        .WithUri(new Uri("http://localhost:11434/v1")) // Local ollama
        .WithModelName("gemma3:12b")
    //.UseTools(...)
    .Build();

string[] inputs = ["Tell me a joke"];

foreach (var input in inputs)
{
    Console.WriteLine($"User: {input}");
    var response = await agent.ProcessInputAsync(input);
    Console.WriteLine($"Agent: {response}\n");
}
