// namespace DotAigent.Models;
//
// using System.Collections.Generic;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using DotAigent.Core;
// using OpenAI;
// using OpenAI.Chat;
//
// /// <summary>
// /// Implementation of the OpenAI model interface that interacts with OpenAI's API.
// /// Provides functionality to generate responses using OpenAI models and execute tools.
// /// </summary>
//
//
// public class OpenAiModel : IModel
// {
//     /// <summary>
//     /// OpenAI client options
//     /// </summary>
//     private readonly OpenAIClientOptions _options = new();
//
//     /// <summary>
//     /// The OpenAI model parameters.
//     /// </summary>
//     private AIModelParameters _modelParameters = new();
//
//     /// Generates a response from the OpenAI model based on the provided prompt.
//     /// </summary>
//     /// <param name="prompt">The input text prompt to send to the language model.</param>
//     /// <returns>A task that resolves to the generated text response from the model.</returns>
//     /// <exception cref="ArgumentException">Thrown when model name is not provided or a tool is not found.</exception>
//     /// <exception cref="NotImplementedException">Thrown when certain finish reasons are encountered but not implemented.</exception>
//     public async Task<AiAgentResponse> GenerateResponseAsync(string prompt, IEnumerable<ITool>? tools, string? systemPrompt = null, string? jsonOutputFormat = null)
//     {
//         var messages = new ChatMessageList();
//         var client = GetClient();
//
//         var modelName = _modelParameters.ModelName ?? throw new InvalidOperationException("You have to provide a model name");
//         systemPrompt ??= tools is not null ? "You are a helpful assistant and use the apropiate tool to solve the problem." : "You are a helpful assistant.";
//
//
//         // First we add the system message to the conversation.
//         messages.AddSystemMessage(string.IsNullOrEmpty(jsonOutputFormat) ? systemPrompt : $"Only output json no other text.\nEXAMPLE OUTPUT: {jsonOutputFormat}");
//
//         // Then we add the user prompt to the conversation.
//         messages.AddUserMessage(prompt);
//
//         // Get the chat client for the specified model.
//         var chatClient = client.GetChatClient(modelName) ?? throw new ArgumentException($"Model not found {modelName}");
//
//         ChatCompletionOptions options = GetChatCompletionOptions(tools, jsonOutputFormat);
//
//         bool requiresAction;
//
//         // Loop until the conversation is complete.
//         do
//         {
//             requiresAction = false;
//             ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);
//
//             switch (completion.FinishReason)
//             {
//                 case ChatFinishReason.Stop:
//                     {
//                         // Add the assistant message to the conversation history.
//                         messages.Add(new AssistantChatMessage(completion));
//                         break;
//                     }
//
//                 case ChatFinishReason.ToolCalls:
//                     {
//                         // First, add the assistant message with tool calls to the conversation history.
//                         messages.Add(new AssistantChatMessage(completion));
//
//                         // Then, add a new tool message for each tool call that is resolved.
//                         foreach (ChatToolCall toolCall in completion.ToolCalls)
//                         {
//                             var tool = tools?.FirstOrDefault(n => n.Name == toolCall.FunctionName) ?? throw new ArgumentException($"Tool not found: {toolCall.FunctionName}");
//
//                             var result = tool switch
//                             {
//                                     IFunctionTool ftool  => await ftool.ExecuteAsync(GetToolParameters(toolCall)),
//                                     _ => throw new ArgumentException($"Tool not found: {toolCall.FunctionName}")
//                             };
//                             messages.Add(new ToolChatMessage(toolCall.Id, result));
//                         }
//
//                         requiresAction = true;
//                         break;
//                     }
//
//                 case ChatFinishReason.Length:
//                     throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");
//
//                 case ChatFinishReason.ContentFilter:
//                     throw new NotImplementedException("Omitted content due to a content filter flag.");
//
//                 case ChatFinishReason.FunctionCall:
//                     throw new NotImplementedException("Deprecated in favor of tool calls.");
//
//                 default:
//                     throw new NotImplementedException(completion.FinishReason.ToString());
//             }
//         } while (requiresAction);
//
//         List<AiChatMessage> responseMessages = FormatResponseMessages(messages);
//         return new AiAgentResponse { Success = true, Messages = responseMessages, Result = responseMessages.Last() };
//     }
//
//     /// <summary>
//     /// Extracts tool parameters from a tool call JSON.
//     /// </summary>
//     /// <param name="toolCall">The tool call containing function arguments.</param>
//     /// <returns>An enumerable collection of tool parameters.</returns>
//     private IEnumerable<ToolParameter> GetToolParameters(ChatToolCall toolCall)
//     {
//         var jsonDocument = JsonDocument.Parse(toolCall.FunctionArguments);
//         // Iterate over the properties of the JSON object.
//         foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
//         {
//             yield return new ToolParameter(property.Name, GetPropertyValue(property.Value));
//         }
//
//
//     }
//
//     /// <summary>
//     /// Converts a JsonElement to its string representation based on its kind.
//     /// </summary>
//     /// <param name="value">The JSON element to convert.</param>
//     /// <returns>String representation of the JSON element.</returns>
//     private string GetPropertyValue(JsonElement value)
//     {
//         return value.ValueKind switch
//         {
//             JsonValueKind.String => value.GetString() ?? "",
//             JsonValueKind.Number => value.GetDouble().ToString(),
//             JsonValueKind.True => "true",
//             JsonValueKind.False => "false",
//             JsonValueKind.Null => "null",
//             _ => value.ToString(),
//         };
//
//     }
//
//     /// <summary>
//     /// Formats the OpenAI chat messages into a list of AiChatMessage objects.
//     /// </summary>
//     /// <param name="messages">The ChatMessageList containing all conversation messages.</param>
//     /// <returns>A list of AiChatMessage objects formatted for the response.</returns>
//     private static List<AiChatMessage> FormatResponseMessages(ChatMessageList messages)
//     {
//         List<AiChatMessage> responseMessages = [];
//         foreach (ChatMessage message in messages)
//         {
//             switch (message)
//             {
//                 case UserChatMessage userMessage:
//                     responseMessages.Add(new AiChatMessage  {Role = ChatRole.User, Message = userMessage.Content[0].Text});
//                     break;
//
//                 case AssistantChatMessage assistantMessage when assistantMessage.Content.Count > 0 && assistantMessage.Content[0].Text.Length > 0:
//                     responseMessages.Add(new AiChatMessage { Role = ChatRole.Agent, Message = assistantMessage.Content[0].Text });
//                     break;
//
//                 case ToolChatMessage tooltMessage when tooltMessage.Content.Count > 0 && tooltMessage.Content[0].Text.Length > 0:
//                     responseMessages.Add(new AiChatMessage { Role = ChatRole.Tool, Message = tooltMessage.Content[0].Text });
//                     break;
//                 case SystemChatMessage systemMessage:
//                     responseMessages.Add(new AiChatMessage { Role = ChatRole.System, Message = systemMessage.Content[0].Text });
//                     break;
//
//                 default:
//                     break;
//             }
//         }
//         return responseMessages;
//     }
//
//     /// <summary>
//     /// Creates and configures chat completion options for the OpenAI API request.
//     /// </summary>
//     /// <returns>Configured chat completion options.</returns>
//     private ChatCompletionOptions GetChatCompletionOptions(IEnumerable<ITool>? tools, string? jsonOutputFormat)
//     {
//         var options = new ChatCompletionOptions();
//         if (tools is not null)
//         {
//             foreach (var tool in tools)
//             {
//                 options.Tools.Add(GetChatTool(tool));
//
//             }
//         }
//         if (jsonOutputFormat is not null)
//             options.ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat();
//         return options;
//     }
//
//     /// <summary>
//     /// Converts an ITool to a ChatTool for use with the OpenAI API.
//     /// </summary>
//     /// <param name="tool">The tool to convert.</param>
//     /// <returns>A ChatTool representation of the input tool.</returns>
//     private static ChatTool GetChatTool(ITool tool)
//     {
//         return ChatTool.CreateFunctionTool(
//             functionName: tool.Name,
//             functionDescription: tool.Description,
//             functionParameters: GetFunctionParameters(tool.Parameters)
//             );
//     }
//
//     /// <summary>
//     /// Converts tool parameter descriptions to a binary data representation for API consumption.
//     /// </summary>
//     /// <param name="parameters">The tool parameter descriptions.</param>
//     /// <returns>Binary data representing the function parameters schema.</returns>
//     private static BinaryData GetFunctionParameters(IEnumerable<ToolParameterDescription> parameters)
//     {
//         if (!parameters.Any())
//             return BinaryData.Empty;
//
//         var required = parameters.Where(n => n.Required).Select(n => n.Name);
//         var properties = parameters.Select(n =>
//                 new KeyValuePair<string, PropertySchema>(n.Name, new PropertySchema { Type = n.Type, Description = n.Description }));
//
//         var schema = new Schema { Required = [.. required], Properties = properties.ToDictionary(n => n.Key, n => n.Value) };
//
//         // Serialize the schema to a JSON string.
//         string json = JsonSerializer.Serialize(schema);
//         return BinaryData.FromString(json);
//
//     }
//
//     /// <summary>
//     /// Creates or returns an existing OpenAI client.
//     /// </summary>
//     /// <returns>An OpenAI client instance.</returns>
//     private OpenAIClient GetClient()
//     {
//         var key = _modelParameters.ApiKey ?? throw new InvalidOperationException("You have to provide an API key");
//         if (_modelParameters.ApiEndpoint is not null)
//             _options.Endpoint = _modelParameters.ApiEndpoint;
//
//         return new OpenAIClient(new(key), options: _options);
//     }
//
//     public void SetModelParameters(AIModelParameters parameters)
//     {
//         _modelParameters = parameters;
//     }
// }
//
// /// <summary>
// /// Represents the schema of a property in a JSON schema.
// /// </summary>
// public record PropertySchema
// {
//     [JsonPropertyName("type")]
//     public string Type { get; init; } = string.Empty;
//
//     [JsonPropertyName("description")]
//     public string Description { get; init; } = string.Empty;
//
// }
//
// /// <summary>
// /// Represents a JSON schema for function parameters.
// /// </summary>
// public record Schema
// {
//     /// <summary>
//     /// Gets the type of the schema. Default is "object".
//     /// </summary>
//     [JsonPropertyName("type")]
//     public string Type { get; init; } = "object";
//
//     /// <summary>
//     /// Gets the dictionary of properties in the schema.
//     /// </summary>
//     [JsonPropertyName("properties")]
//     public Dictionary<string, PropertySchema> Properties { get; init; } = [];
//
//     /// <summary>
//     /// Gets the array of required property names.
//     /// </summary>
//     [JsonPropertyName("required")]
//     public string[] Required { get; init; } = [];
// }
