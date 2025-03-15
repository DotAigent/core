namespace DotAigent.Core;

public interface IModel
{
    Task<string> GenerateResponseAsync(string prompt);
}
