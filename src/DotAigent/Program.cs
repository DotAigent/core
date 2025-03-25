using DotAigent.Core;
using DotAigent.Providers.OpenAi;
using DotNetEnv;

internal partial class Program
{
    private static async Task Main(string[] args)
    {
        Env.Load();

        Console.WriteLine(AgentSumOutput.JsonExample);
        
    }
}


// var aent = new AgentBuilder2()
//     .WithSystemPrompt("some systemprompt")
//     .WithResultType<AgentDataOutput>()
//     .UsingTool(new GoogleSearchTool())
//     .UsingProvider(Provider.OpenAI)
//         .WithEndpoint(new Uri("http://localhost:11434"))
//         .WithModel("llama3.2:latest")
//     .Build();



internal class GoogleSearchTool : ITool
{
    public GoogleSearchTool()
    {
    }

    public string Name => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public IEnumerable<ToolParameterDescription> Parameters => throw new NotImplementedException();
}


// Agent
//     - SystemPrompt
//     - Tools
//     - StructuredResultType
//     - Model
//     - ModelSettings
//
// Model
//     - ModelName
//     - Provider (the service provider of the Model)
//     - Interface (The API endpoint type)
//
// Tool
//     - Structured Input
//     - ExecuteTool
//     - Structured Output
//
