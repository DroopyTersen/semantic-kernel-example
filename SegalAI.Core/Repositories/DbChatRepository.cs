using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace SegalAI.Core.Repositories;

public class DbChatRepository : IChatRepository
{
  public IEnumerable<ChatMessageContent> LoadConversation(string conversationId)
  {
    // TODO: Implement database loading logic
    // Example pseudo-code:
    // return _dbContext.Messages
    //     .Where(m => m.ConversationId == conversationId)
    //     .OrderBy(m => m.Timestamp)
    //     .Select(m => new ChatMessage(m.Role, m.Content));

    throw new NotImplementedException();
  }

  public Task SaveMessage(string conversationId, ChatMessageContent message)
  {
    // TODO: Implement database saving logic
    // Example pseudo-code:
    // await _dbContext.Messages.AddAsync(new MessageEntity
    // {
    //     ConversationId = conversationId,
    //     Role = message.Role,
    //     Content = message.Content,
    //     Timestamp = message.Timestamp
    // });
    // await _dbContext.SaveChangesAsync();

    throw new NotImplementedException();
  }
}