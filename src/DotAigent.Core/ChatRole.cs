namespace DotAigent.Core;

/// <summary>
/// The role that is the origin of the message.
/// </summary>
public enum ChatRole
{

    /// <summary>
    /// The message is a system message.
    /// </summary>
    System,
    /// <summary>
    /// The message originated from the user.
    /// </summary>
    User,
    /// <summary>
    /// The message originated from the agent.
    /// </summary>
    Agent,
    /// <summary>
    /// The message originated from a tool.
    /// </summary>
    Tool
}

