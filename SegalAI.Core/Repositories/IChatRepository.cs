using System.Collections.Generic;
using System.Threading.Tasks;
using SegalAI.Core.Models;

namespace SegalAI.Core.Repositories;

public interface IChatRepository
{
  IEnumerable<ChatMessage> LoadConversation(string conversationId);
  Task SaveMessage(string conversationId, ChatMessage message);
}