namespace DotAigent.Models;

using OpenAI;
using OpenAI.Chat;


public class OpenAiModel : IOpenAiModel
{
    private const string ParamName = "Model name is required";
    private readonly OpenAIClientOptions _options = new();
    public OpenAiModel(string? apiKey = null, string? modelName = null, Uri? uri= null)
    {
        ApiKey = apiKey ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "provide-your-api-key-please";
        Uri = uri;
        ModelName = modelName ?? Environment.GetEnvironmentVariable("OPENAI_DEFAULT_MODEL") ?? "";

    }

    public string ApiKey { get;  set; }

    public string ModelName { get ;  set;  }
    public Uri? Uri { get;  set; }

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        if (string.IsNullOrEmpty(ModelName))
            throw new ArgumentException(ParamName);

        if (Uri is not null)
            _options.Endpoint = Uri;

        Console.WriteLine($"Using model: {ModelName}, with uri: {Uri}");
        var client = new ChatClient(model: ModelName, credential: new (ApiKey), options: _options);

        ChatCompletion completion = await client.CompleteChatAsync(prompt);

        return completion.Content[0].Text;
    }
}
