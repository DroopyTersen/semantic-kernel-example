using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Repositories;

public class InMemoryChatRepository : IChatRepository
{
  private readonly Dictionary<string, List<ChatMessage>> _conversations = new();

  public IEnumerable<ChatMessage> LoadConversation(string conversationId)
  {
    return _conversations.TryGetValue(conversationId, out var messages)
        ? messages
        : Enumerable.Empty<ChatMessage>();
  }

  public Task SaveMessage(string conversationId, ChatMessage message)
  {
    if (!_conversations.ContainsKey(conversationId))
    {
      _conversations[conversationId] = new List<ChatMessage>();
    }

    _conversations[conversationId].Add(message);
    return Task.CompletedTask;
  }
}