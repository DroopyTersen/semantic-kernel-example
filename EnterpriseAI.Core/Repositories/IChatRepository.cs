using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Repositories;

/// <summary>
/// Manages chat conversations and message history.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Loads all messages for a given conversation.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <returns>An enumerable of chat messages ordered by timestamp.</returns>
    Task<IEnumerable<ChatMessage>> LoadConversationAsync(string conversationId);

    /// <summary>
    /// Saves a new message to the specified conversation.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="message">The chat message to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveMessageAsync(string conversationId, ChatMessage message);
}
