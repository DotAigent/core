// using DotAigent.Core.Agents;
//
// namespace DotAigent.Core;
//
// /// <summary>
// /// Builder for creating agent instances
// /// </summary>
// public class AgentBuilder : IAgentBuilder
// {
//     private IModel? _model;
//     private string _systemPrompt = string.Empty;
//
//     /// <summary>
//     /// Set the AI model to use
//     /// </summary>
//     public IAgentBuilder WithModel(IModel model)
//     {
//         _model = model ?? throw new ArgumentNullException(nameof(model));
//         return this;
//     }
//
//     /// <summary>
//     /// Set the AI model to use
//     /// </summary>
//     public IAgentBuilder WithSystemPrompt(string systemPrompt)
//     {
//         if (string.IsNullOrEmpty(systemPrompt))
//                 throw new ArgumentException("System prompt cannot be null or empty.", nameof(systemPrompt));
//         _systemPrompt = systemPrompt;
//         return this;
//     }
//
//     /// <summary>
//     /// Build the agent 
//     /// </summary>
//     public IAgent Build()
//     {
//         if (_model == null)
//             throw new InvalidOperationException("An AI model must be specified before building the agent."); 
//
//
//         return new ChatbotAgent(_model, []);
//     }
// }
