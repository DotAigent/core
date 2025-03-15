namespace DotAigent.Models;

using System.Collections.Generic;
using DotAigent.Core;

public class OpenAiAgentBuilder : IToolAgentBuilder
{
    private readonly IAgentBuilder _builder;
    readonly OpenAiModel _agent = new();

    public OpenAiAgentBuilder(IAgentBuilder builder)
    {
        _builder = builder;
        _builder.WithModel(_agent);
    }

    public IAgent Build()
    {
        return _builder.Build();
    }

    public IToolAgentBuilder WithJsonOutputFormat(string jsonOutputFormat)
    {
        _agent.JsonOutputFormat = jsonOutputFormat;
        return this;
    }

    public OpenAiAgentBuilder WithKey(string key)
    {
        _agent.ApiKey = key;
        return this;
    }

    public IAgentBuilder WithModel(IModel model)
    {
        return _builder.WithModel(model);
    }

    public OpenAiAgentBuilder WithModelName(string modelName)
    {
        _agent.ModelName = modelName;
        return this;
    }

    public IAgentBuilder WithSystemPrompt(string systemPrompt)
    {
        _builder.WithSystemPrompt(systemPrompt);
        return this;
    }

    public IToolAgentBuilder WithTool(ITool tool)
    {
        _agent.Tools.Add(tool);
        return this;
    }

    public IToolAgentBuilder WithTools(IEnumerable<ITool> tools)
    {
        _agent.Tools.AddRange(tools);
        return this;
    }

    public OpenAiAgentBuilder WithUri(Uri uri)
    {
        _agent.Uri = uri;
        return this;
    }
}


