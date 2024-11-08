using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Repositories;

public class InMemoryChatRepository : IChatRepository
{
    private readonly Dictionary<string, List<ChatMessage>> _conversations = [];

    public Task SaveMessageAsync(string conversationId, ChatMessage message)
    {
        if (!_conversations.ContainsKey(conversationId))
        {
            _conversations[conversationId] = [];
        }

        _conversations[conversationId].Add(message);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ChatMessage>> LoadConversationAsync(string conversationId)
    {
        return Task.FromResult(_conversations.TryGetValue(conversationId, out var messages)
            ? (IEnumerable<ChatMessage>)messages
            : Enumerable.Empty<ChatMessage>());
    }

}