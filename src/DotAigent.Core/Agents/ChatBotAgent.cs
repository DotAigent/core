

namespace DotAigent.Core.Agents;

// public class ChatbotAgent(IModel model, IEnumerable<ITool> tools, string? systemPrompt = null, string? outputFormat=null) : IAgent
// {
//     private readonly IEnumerable<ITool> _tools = tools;
//
//     public IEnumerable<ITool> Tools =>  _tools;
//
//     public async Task<AiAgentResponse> GenerateResponseAsync(string prompt)
//     {
//         try
//         {
//             return await model.GenerateResponseAsync(prompt, _tools, systemPrompt, outputFormat);
//         }
//         catch (Exception ex)
//         {
//             return new AiAgentResponse
//             {
//                 Success = false,
//                 ErrorMessage = ex.Message
//             };
//         }
//     }
//
// }



