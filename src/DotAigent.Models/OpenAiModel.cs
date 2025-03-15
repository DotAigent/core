namespace DotAigent.Models;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotAigent.Core;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;


public class OpenAiModel : IOpenAiModel
{
    private const string ParamName = "Model name is required";
    private readonly OpenAIClientOptions _options = new();
    private OpenAIClient? _client;
    public OpenAiModel(string? apiKey = null, string? modelName = null, Uri? uri = null)
    {
        ApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "provide-your-api-key-please";
        Uri = uri;
        ModelName = modelName ?? Environment.GetEnvironmentVariable("OPENAI_DEFAULT_MODEL") ?? "";

    }

    public string ApiKey { get; set; }

    public string ModelName { get; set; }
    public Uri? Uri { get; set; }
    public List<ITool> Tools { get; } = [];

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        _client ??= GetClient();

        if (string.IsNullOrEmpty(ModelName))
            throw new ArgumentException(ParamName);

        var messages = new List<ChatMessage>();
        var promtMessage = new UserChatMessage(prompt);

        messages.Add(promtMessage);

        var client = _client.GetChatClient(ModelName) ?? throw new ArgumentException($"Model not found {ModelName}");
        ChatCompletionOptions options = GetChatCompletionOptions();

        bool requiresAction;
        do
        {
            requiresAction = false;
            ChatCompletion completion = await client.CompleteChatAsync(messages, options);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        // Add the assistant message to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        // First, add the assistant message with tool calls to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));

                        // Then, add a new tool message for each tool call that is resolved.
                        foreach (ChatToolCall toolCall in completion.ToolCalls)
                        {
                            var tool = Tools.FirstOrDefault(n => n.Name == toolCall.FunctionName) ?? throw new ArgumentException($"Tool not found: {toolCall.FunctionName}");
                            var result = await tool.ExecuteAsync(GetToolParameters(toolCall));

                            messages.Add(new ToolChatMessage(toolCall.Id, result));
                        }

                        requiresAction = true;
                        break;
                    }

                case ChatFinishReason.Length:
                    throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                case ChatFinishReason.ContentFilter:
                    throw new NotImplementedException("Omitted content due to a content filter flag.");

                case ChatFinishReason.FunctionCall:
                    throw new NotImplementedException("Deprecated in favor of tool calls.");

                default:
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (requiresAction);

        var sw = new StringWriter();
        foreach (ChatMessage message in messages)
        {
            switch (message)
            {
                case UserChatMessage userMessage:
                    sw.WriteLine($"[USER]:");
                    sw.WriteLine($"{userMessage.Content[0].Text}");
                    sw.WriteLine();
                    break;

                case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0 && assistantMessage.Content[0].Text.Length > 0:
                    sw.WriteLine($"[ASSISTANT]:");
                    sw.WriteLine($"{assistantMessage.Content[0].Text}");
                    sw.WriteLine();
                    break;

                case ToolChatMessage:
                    // Do not print any tool messages; let the assistant summarize the tool results instead.
                    break;

                default:
                    break;
            }
        }
        return sw.ToString();
    }

    private IEnumerable<ToolParameter> GetToolParameters(ChatToolCall toolCall)
    {
        var jsonDocument = JsonDocument.Parse(toolCall.FunctionArguments);
        // Iterate over the properties of the JSON object.
        foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
        {
            yield return new ToolParameter(property.Name, GetPropertyValue(property.Value));
        }


    }

    private string GetPropertyValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? "",
            JsonValueKind.Number => value.GetDouble().ToString(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "null",
            _ => value.ToString(),
        };

    }

    private ChatCompletionOptions GetChatCompletionOptions()
    {
        var options = new ChatCompletionOptions();
        foreach (var tool in Tools)
        {
            options.Tools.Add(GetChatTool(tool));

        }
        return options;
    }

    private static ChatTool GetChatTool(ITool tool)
    {
        return ChatTool.CreateFunctionTool(
            functionName: tool.Name,
            functionDescription: tool.Description,
            functionParameters: GetFunctionParameters(tool.Parameters)
            );
    }

    private static BinaryData GetFunctionParameters(IEnumerable<ToolParameterDescription> parameters)
    {
       if (!parameters.Any())
           return BinaryData.Empty;
    
       var required = parameters.Where(n => n.Required).Select(n => n.Name);
       var properties = parameters.Select(n => 
               new KeyValuePair<string, PropertySchema>(n.Name, new PropertySchema { Type = n.Type, Description = n.Description }));

       var schema = new Schema { Required = [.. required], Properties = properties.ToDictionary(n => n.Key, n => n.Value) };
       
       // Serialize the schema to a JSON string.
       string json = JsonSerializer.Serialize(schema);
       return BinaryData.FromString(json);

    }

    private OpenAIClient GetClient()
    {
        if (Uri is not null)
            _options.Endpoint = Uri;

        return new OpenAIClient(new(ApiKey), options: _options);
    }
}
public record PropertySchema
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

}

public record Schema
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertySchema> Properties { get; init; } = [];

    [JsonPropertyName("required")]
    public string[] Required { get; init; } = [];
}
