namespace DotAigent.Models;

using DotAigent.Core;
using OpenAI;

/// <summary>
/// Interface for OpenAI model implementations
/// </summary>
public interface IOpenAiModel : IModel
{
    /// <summary>
    /// Gets or sets the model name to use
    /// </summary>
    string ModelName { get; internal set; }

    string ApiKey { get; internal set; }
    Uri? Uri { get; internal set; }

}

