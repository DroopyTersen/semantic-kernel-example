using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Repositories;

public interface IChatRepository
{
  IEnumerable<ChatMessage> LoadConversation(string conversationId);
  Task SaveMessage(string conversationId, ChatMessage message);
}