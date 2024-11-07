using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace SegalAI.Core.Repositories;

public interface IChatRepository
{
  IEnumerable<ChatMessageContent> LoadConversation(string conversationId);
  Task SaveMessage(string conversationId, ChatMessageContent message);
}