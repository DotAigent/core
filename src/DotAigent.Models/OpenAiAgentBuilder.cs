namespace DotAigent.Models;

using System.Collections.Generic;
using DotAigent.Core;
using DotAigent.Core.Agents;

public class OpenAiAgentBuilder : IToolAgentBuilder
{
    private string? _outputFormat;
    private string? _apiKey;
    private string? _modelName;
    private string? _systemPrompt;
    private Uri? _apiEndpoint;
    private readonly List<ITool> _tools = [];


    public OpenAiAgentBuilder()
    {
    }

    public IAgent Build()
    {
        var model = new OpenAiModel();

        // Get key from environment variable if not set
        _apiKey ??= Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        model.SetModelParameters(
                new AIModelParameters
                {
                    ApiKey = _apiKey,
                    ModelName = _modelName,
                    ApiEndpoint = _apiEndpoint,

                });

        var agent = new ChatbotAgent(model, _tools, _systemPrompt, _outputFormat);

        return agent;
    }

    public IToolAgentBuilder WithJsonOutputFormat(string outputFormat)
    {
        _outputFormat = outputFormat;
        return this;
    }

    public OpenAiAgentBuilder WithKey(string key)
    {
        _apiKey = key;
        return this;
    }

    public OpenAiAgentBuilder WithModelName(string modelName)
    {
        _modelName = modelName;
        return this;
    }

    public IAgentBuilder WithSystemPrompt(string systemPrompt)
    {
        _systemPrompt = systemPrompt;
        return this;
    }

    public IToolAgentBuilder WithTool(ITool tool)
    {
        _tools.Add(tool);
        return this;
    }

    public IToolAgentBuilder WithTools(IEnumerable<ITool> tools)
    {
        _tools.AddRange(tools);
        return this;
    }

    public IToolAgentBuilder WithUri(Uri uri)
    {
        _apiEndpoint = uri;
        return this;
    }

    public IToolAgentBuilder WithToolAgent(IAgent additionTookAgent)
    {
        throw new NotImplementedException();
    }
}


