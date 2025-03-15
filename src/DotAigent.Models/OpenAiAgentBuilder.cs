namespace DotAigent.Models;

using System.Collections.Generic;
using DotAigent.Core;

public class OpenAiAgentBuilder : IAgentBuilder
{
    private readonly IAgentBuilder _builder;
    OpenAiModel _agent = new();

    public OpenAiAgentBuilder(IAgentBuilder builder)
    {
        _builder = builder;
        _builder.WithModel(_agent);
    }

    public IAgent Build()
    {
        return _builder.Build();
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
        Console.WriteLine("Setting model name to: " + modelName);
        _agent.ModelName = modelName;
        return this;
    }

    public IAgentBuilder WithTool(ITool tool)
    {
        return _builder.WithTool(tool);
    }

    public IAgentBuilder WithTools(IEnumerable<ITool> tools)
    {
        return _builder.WithTools(tools);
    }

    public OpenAiAgentBuilder WithUri(Uri uri)
    {
        _agent.Uri = uri;
        return this;
    }
}


