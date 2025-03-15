namespace DotAigent.Core;

using System.Text.Json;

public interface IAgent
{
    Task<string> ProcessInputAsync(string input);
    /*/// <summary>*/
    /*/// Process the agent asynchronously without returning a specific result.*/
    /*/// </summary>*/
    /*public Task<string> ProcessAsync();*/
    /**/
    /*/// <summary>*/
    /*/// Process the agent asynchronously and returns a result of type T.*/
    /*/// </summary>*/
    /*/// <typeparam name="T">The type of result to return.</typeparam>*/
    /*/// <returns>A task representing the asynchronous operation with the result of type T.</returns>*/
    /*public Task<T> ProcessAsync<T>();*/
    /**/
    /*/// <summary>*/
    /*/// Process the agent asynchronously and returns a JsonElement result.*/
    /*/// </summary>*/
    /*/// <returns>A task representing the asynchronous operation with the result as a JsonElement.</returns>*/
    /*public Task<JsonElement> ProcessAsJsonAsync();*/
}
