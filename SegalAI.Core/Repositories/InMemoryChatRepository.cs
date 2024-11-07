using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using SegalAI.Core.Repositories;

public class InMemoryChatRepository : IChatRepository
{
  private readonly Dictionary<string, List<ChatMessageContent>> _conversations = new();

  public IEnumerable<ChatMessageContent> LoadConversation(string conversationId)
  {
    return _conversations.TryGetValue(conversationId, out var messages)
        ? messages
        : Enumerable.Empty<ChatMessageContent>();
  }

  public Task SaveMessage(string conversationId, ChatMessageContent message)
  {
    if (!_conversations.ContainsKey(conversationId))
    {
      _conversations[conversationId] = new List<ChatMessageContent>();
    }

    _conversations[conversationId].Add(message);
    return Task.CompletedTask;
  }
}