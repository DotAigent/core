namespace DotAigent.Providers.OpenAi;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotAigent.Core;
using OpenAI;
using OpenAI.Chat;

public class OpenAIProvider(string modelName, string? apiKey = null, Uri? endpoint = null) : IProvider
{
    public string ModelName => modelName; 

    public Uri? Endpoint => endpoint;


    public async Task<IAgentResponse<T>> GenerateResponseAsync<T>(string prompt, string? systemPrompt, IEnumerable<ITool>? tools) where T : class
    {
        var messages = new ChatMessageList();
        var client = GetClient();
        string? jsonOutputFormat = null;

        systemPrompt ??= tools is not null ? "You are a helpful assistant and use the apropiate tool to solve the problem." : "You are a helpful assistant.";

        if (typeof(T) != typeof(string))
        {
            // We have a structured output. Lets get the output example json from type T
                // If no parameterless constructor exists (common with records), create a sample JSON structure
                var properties = typeof(T).GetProperties();
                var sampleObject = new Dictionary<string, object?>();
                
                foreach (var prop in properties)
                {
                    // Add sample values based on property type
                    var value = GetDefaultValueForType(prop.PropertyType);
                    sampleObject[prop.Name] = value;
                }
                jsonOutputFormat = JsonSerializer.Serialize(sampleObject);
        }

        // First we add the system message to the conversation.
        messages.AddSystemMessage(jsonOutputFormat is null ? systemPrompt : $"Only output json no other text.\nEXAMPLE OUTPUT: {jsonOutputFormat}");

        // Then we add the user prompt to the conversation.
        messages.AddUserMessage(prompt);

        // Get the chat client for the specified model.
        var chatClient = client.GetChatClient(modelName) ?? throw new ArgumentException($"Model not found {modelName}");

        ChatCompletionOptions options = GetChatCompletionOptions(tools, jsonOutputFormat);

        bool requiresAction;

        // Loop until the conversation is complete.
        do
        {
            requiresAction = false;
            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

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
                            var tool = tools?.FirstOrDefault(n => n.Name == toolCall.FunctionName) ?? throw new ArgumentException($"Tool not found: {toolCall.FunctionName}");

                            var result = tool switch
                            {
                                    IFunctionTool ftool  => await ftool.ExecuteAsync(GetToolParameters(toolCall)),
                                    _ => throw new ArgumentException($"Tool not found: {toolCall.FunctionName}")
                            };
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

        List<Core.ChatMessage> responseMessages = FormatResponseMessages(messages);
        return new AgentResponse<T> { Success = true, Messages = responseMessages, Result = GetResult<T>(messages.Last()) };
    }

    /// <summary>
    /// Gets a default sample value for a given type to use in JSON structure examples.
    /// </summary>
    /// <param name="type">The type to generate a sample value for.</param>
    /// <returns>A sample value appropriate for the type.</returns>
    private static object? GetDefaultValueForType(Type type)
    {
        if (type == typeof(string))
            return "sample text";
        else if (type == typeof(int) || type == typeof(int?))
            return 0;
        else if (type == typeof(long) || type == typeof(long?))
            return 0L;
        else if (type == typeof(double) || type == typeof(double?))
            return 0.0;
        else if (type == typeof(decimal) || type == typeof(decimal?))
            return 0.0m;
        else if (type == typeof(bool) || type == typeof(bool?))
            return false;
        else if (type == typeof(DateTime) || type == typeof(DateTime?))
            return DateTime.Now.ToString("o");
        else if (type == typeof(Guid) || type == typeof(Guid?))
            return Guid.Empty.ToString();
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return Activator.CreateInstance(type);
        else if (type.IsEnum)
            return Enum.GetNames(type).FirstOrDefault() ?? Enum.GetValues(type).GetValue(0)?.ToString();
        else if (type.IsClass && type != typeof(object))
        {
            try
            {
                // For nested complex types, we return null to avoid infinite recursion
                return null;
            }
            catch
            {
                return null;
            }
        }
        else
            return null;
    }
    private T? GetResult<T>(OpenAI.Chat.ChatMessage completion)
    {
        var result = completion.Content.FirstOrDefault()?.Text;

        if (typeof(T) == typeof(string))
        {
            return (T?)(object?)result;
        }
        else
        {
            if (result is not null)
                return JsonSerializer.Deserialize<T>(result);
            else 
                return (T?)(object?)null;
        }
    }

    /// <summary>
    /// Extracts tool parameters from a tool call JSON.
    /// </summary>
    /// <param name="toolCall">The tool call containing function arguments.</param>
    /// <returns>An enumerable collection of tool parameters.</returns>
    private IEnumerable<ToolParameter> GetToolParameters(ChatToolCall toolCall)
    {
        var jsonDocument = JsonDocument.Parse(toolCall.FunctionArguments);
        // Iterate over the properties of the JSON object.
        foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
        {
            yield return new ToolParameter(property.Name, GetPropertyValue(property.Value));
        }


    }

    /// <summary>
    /// Converts a JsonElement to its string representation based on its kind.
    /// </summary>
    /// <param name="value">The JSON element to convert.</param>
    /// <returns>String representation of the JSON element.</returns>
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

    /// <summary>
    /// Formats the OpenAI chat messages into a list of AiChatMessage objects.
    /// </summary>
    /// <param name="messages">The ChatMessageList containing all conversation messages.</param>
    /// <returns>A list of AiChatMessage objects formatted for the response.</returns>
    private static List<Core.ChatMessage> FormatResponseMessages(ChatMessageList messages)
    {
        List<Core.ChatMessage> responseMessages = [];
        foreach (OpenAI.Chat.ChatMessage message in messages)
        {
            switch (message)
            {
                case UserChatMessage userMessage:
                    responseMessages.Add(new Core.ChatMessage  { Role = ChatRole.User, Message = userMessage.Content[0].Text});
                    break;

                case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0 && assistantMessage.Content[0].Text.Length > 0:
                    responseMessages.Add(new Core.ChatMessage { Role = ChatRole.Agent, Message = assistantMessage.Content[0].Text });
                    break;

                case ToolChatMessage tooltMessage when tooltMessage.Content.Count > 0 && tooltMessage.Content[0].Text.Length > 0:
                    responseMessages.Add(new Core.ChatMessage { Role = ChatRole.Tool, Message = tooltMessage.Content[0].Text });
                    break;
                case SystemChatMessage systemMessage:
                    responseMessages.Add(new Core.ChatMessage { Role = ChatRole.System, Message = systemMessage.Content[0].Text });
                    break;

                default:
                    break;
            }
        }
        return responseMessages;
    }

    /// <summary>
    /// Creates and configures chat completion options for the OpenAI API request.
    /// </summary>
    /// <returns>Configured chat completion options.</returns>
    private ChatCompletionOptions GetChatCompletionOptions(IEnumerable<ITool>? tools, string? jsonOutputFormat)
    {
        var options = new ChatCompletionOptions();
        if (tools is not null)
        {
            foreach (var tool in tools)
            {
                options.Tools.Add(GetChatTool(tool));
            }
        }
        if (jsonOutputFormat is not null)
            options.ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat();
        return options;
    }

    /// <summary>
    /// Converts an ITool to a ChatTool for use with the OpenAI API.
    /// </summary>
    /// <param name="tool">The tool to convert.</param>
    /// <returns>A ChatTool representation of the input tool.</returns>
    private static ChatTool GetChatTool(ITool tool)
    {
        return ChatTool.CreateFunctionTool(
            functionName: tool.Name,
            functionDescription: tool.Description,
            functionParameters: GetFunctionParameters(tool.Parameters)
            );
    }

    /// <summary>
    /// Converts tool parameter descriptions to a binary data representation for API consumption.
    /// </summary>
    /// <param name="parameters">The tool parameter descriptions.</param>
    /// <returns>Binary data representing the function parameters schema.</returns>
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

    /// <summary>
    /// Creates or returns an existing OpenAI client.
    /// </summary>
    /// <returns>An OpenAI client instance.</returns>
    private OpenAIClient GetClient()
    {
        var options = new OpenAIClientOptions();
        var key = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("You have to provide an API key");

        if (Endpoint is not null)
            options.Endpoint = Endpoint;

        return new OpenAIClient(new(key), options: options);
    }
}

/// <summary>
/// Represents the schema of a property in a JSON schema.
/// </summary>
public record PropertySchema
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

}

/// <summary>
/// Represents a JSON schema for function parameters.
/// </summary>
public record Schema
{
    /// <summary>
    /// Gets the type of the schema. Default is "object".
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "object";

    /// <summary>
    /// Gets the dictionary of properties in the schema.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, PropertySchema> Properties { get; init; } = [];

    /// <summary>
    /// Gets the array of required property names.
    /// </summary>
    [JsonPropertyName("required")]
    public string[] Required { get; init; } = [];
}
