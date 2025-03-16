namespace DotAigent.Models;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotAigent.Core;
using OpenAI;
using OpenAI.Chat;

/// <summary>
/// Implementation of the OpenAI model interface that interacts with OpenAI's API.
/// Provides functionality to generate responses using OpenAI models and execute tools.
/// </summary>


public class OpenAiModel : IOpenAiModel
{
    /// <summary>
    /// Error message used when model name is not provided
    /// </summary>
    private const string ParamName = "Model name is required";
    
    /// <summary>
    /// OpenAI client options
    /// </summary>
    private readonly OpenAIClientOptions _options = new();
    
    /// <summary>
    /// OpenAI client instance
    /// </summary>
    private OpenAIClient? _client;
    
    /// <summary>
    /// System prompt that provides context to the model
    /// </summary>
    private string _systemPrompt = string.Empty;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAiModel"/> class.
    /// </summary>
    /// <param name="apiKey">The API key for accessing OpenAI API. If null, will use OPENAI_API_KEY environment variable.</param>
    /// <param name="modelName">The name of the model to use. If null, will use OPENAI_DEFAULT_MODEL environment variable.</param>
    /// <param name="uri">The URI for the API endpoint. If null, will use OpenAI's default endpoint.</param>
    public OpenAiModel(string? apiKey = null, string? modelName = null, Uri? uri = null)
    {
        ApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "provide-your-api-key-please";
        Uri = uri;
        ModelName = modelName ?? Environment.GetEnvironmentVariable("OPENAI_DEFAULT_MODEL") ?? "";
    }

    /// <summary>
    /// Gets or sets the API key for accessing the OpenAI API.
    /// </summary>
    public string ApiKey { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the OpenAI model to use.
    /// </summary>
    public string ModelName { get; set; }
    
    /// <summary>
    /// Gets or sets the URI for the API endpoint.
    /// </summary>
    public Uri? Uri { get; set; }
    
    /// <summary>
    /// Gets the list of tools available to the model.
    /// </summary>
    public List<ITool> Tools { get; } = [];
    
    /// <summary>
    /// Gets or sets the JSON format expected in the output.
    /// </summary>
    public string JsonOutputFormat { get; internal set; }

    /// <summary>
    /// Generates a response from the OpenAI model based on the provided prompt.
    /// </summary>
    /// <param name="prompt">The input text prompt to send to the language model.</param>
    /// <returns>A task that resolves to the generated text response from the model.</returns>
    /// <exception cref="ArgumentException">Thrown when model name is not provided or a tool is not found.</exception>
    /// <exception cref="NotImplementedException">Thrown when certain finish reasons are encountered but not implemented.</exception>
    public async Task<string> GenerateResponseAsync(string prompt)
    {
        _client ??= GetClient();

        if (string.IsNullOrEmpty(ModelName))
            throw new ArgumentException(ParamName);

        if (string.IsNullOrEmpty(_systemPrompt))
            SetSystemPrompt(string.Empty);

        var messages = new List<ChatMessage>();

        var systemPromptMessage = new SystemChatMessage(string.IsNullOrEmpty(JsonOutputFormat) ? _systemPrompt : $"Only output json no other text.\nEXAMPLE OUTPUT: {JsonOutputFormat}");
        messages.Add(systemPromptMessage);

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
                case SystemChatMessage systemMessage:
                    sw.WriteLine($"[SYSTEM]:");
                    sw.WriteLine($"{systemMessage.Content[0].Text}");
                    sw.WriteLine();
                    break;

                default:
                    break;
            }
        }
        return sw.ToString();
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
    /// Creates and configures chat completion options for the OpenAI API request.
    /// </summary>
    /// <returns>Configured chat completion options.</returns>
    private ChatCompletionOptions GetChatCompletionOptions()
    {
        var options = new ChatCompletionOptions();
        foreach (var tool in Tools)
        {
            options.Tools.Add(GetChatTool(tool));

        }
        if (string.IsNullOrEmpty(JsonOutputFormat))
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
        if (Uri is not null)
            _options.Endpoint = Uri;

        return new OpenAIClient(new(ApiKey), options: _options);
    }

    /// <summary>
    /// Sets the system prompt for the model.
    /// </summary>
    /// <param name="systemPrompt">The system prompt to set for the model.</param>
    /// <remarks>
    /// If an empty string is provided, a default system prompt will be used based on whether tools are available.
    /// </remarks>
    public void SetSystemPrompt(string systemPrompt)
    {
        if (string.IsNullOrEmpty(systemPrompt))
        {
            _systemPrompt = Tools.Count > 0 ? "You are a helpful assistant and use the apropiate tool to solve the problem." : "You are a helpful assistant.";
        }
        _systemPrompt = systemPrompt;
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
