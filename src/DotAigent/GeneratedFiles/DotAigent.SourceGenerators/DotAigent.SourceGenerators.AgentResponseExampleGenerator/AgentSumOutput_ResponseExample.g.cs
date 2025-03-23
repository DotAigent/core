using DotAigent.Core;

public partial class AgentSumOutput : IExample
{
    private static string _jsonExample => @"{
  ""Sum"": 42
}";
    /// <summary>
    /// Example JSON representation of this class
    /// </summary>
    public string JsonExample => AgentSumOutput._jsonExample;
}
