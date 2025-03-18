using DotAigent.Core;
using DotAigent.Models;
using DotAigent.Tools;
using DotNetEnv;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

Env.Load();

// string[] Scopes = [CalendarService.Scope.Calendar];
// string ApplicationName = "Slaven";
//
// GoogleCredential credential;
// using (var stream = new FileStream("slaven-secret.json", FileMode.Open, FileAccess.Read))
// {
//     credential = GoogleCredential.FromStream(stream)
//     .CreateScoped(Scopes);
//     /*.CreateWithUser("tomash277@gmail.com");*/
// }
//
// // Create Google Calendar API service
// var service = new CalendarService(new BaseClientService.Initializer()
// {
//     HttpClientInitializer = credential,
//     ApplicationName = ApplicationName,
// });
// string calendarId = "07to468qci29lvcgkpi9vks6ek@group.calendar.google.com";
// var request = service.Events.List(calendarId);
// request.TimeMinDateTimeOffset = DateTime.Now;
// var events = request.Execute();
//
// Console.WriteLine("Events retrieved successfully!");
// Console.WriteLine($"Total events: {events.Items?.Count ?? 0}");
// Console.WriteLine("Upcoming events:");
// if (events.Items != null)
// {
//     foreach (var eventItem in events.Items)
//     {
//         Console.WriteLine($"{eventItem.Summary} ({eventItem.Start.DateTime})");
//     }
// }

var agent = new AgentBuilder2()
    .UsingProvider(new OpenAIProvider("gpt-4o-mini"))
        .Build();

var response = await agent.GenerateResponseAsync<AgentSumOutput>("What is the sum of 1 and 2");
Console.WriteLine(response.Result);

public record AgentSumOutput(int Sum);

// foreach (var message in result.Messages)
// {
//     Console.WriteLine(message);
// }

// var aent = new AgentBuilder2()
//     .WithSystemPrompt("some systemprompt")
//     .WithResultType<AgentDataOutput>()
//     .UsingTool(new GoogleSearchTool())
//     .UsingProvider(Provider.OpenAI)
//         .WithEndpoint(new Uri("http://localhost:11434"))
//         .WithModel("llama3.2:latest")
//     .Build();

public record AgentDataOutput(string query);


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
