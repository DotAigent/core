namespace DotAigent.Core;

public interface IProvider
{
    string ModelName { get; }
    Uri? Endpoint { get; }
}
