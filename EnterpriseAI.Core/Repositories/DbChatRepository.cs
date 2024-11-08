using EnterpriseAI.Core.Models;

namespace EnterpriseAI.Core.Repositories;

public class DbChatRepository : IChatRepository
{
    public Task<IEnumerable<ChatMessage>> LoadConversationAsync(string conversationId)
    {
        // TODO: Implement database loading logic
        // Example pseudo-code:
        // return _dbContext.Messages
        //     .Where(m => m.ConversationId == conversationId)
        //     .OrderBy(m => m.Timestamp)
        //     .Select(m => new ChatMessage(
        //         Enum.Parse<MessageRole>(m.Role),
        //         m.Content
        //     ));

        throw new NotImplementedException();
    }

    public Task SaveMessageAsync(string conversationId, ChatMessage message)
    {
        // TODO: Implement database saving logic
        // Example pseudo-code:
        // await _dbContext.Messages.AddAsync(new MessageEntity
        // {
        //     ConversationId = conversationId,
        //     Role = message.Role.ToString(),
        //     Content = message.Content,
        //     Timestamp = message.Timestamp
        // });
        // await _dbContext.SaveChangesAsync();

        throw new NotImplementedException();
    }
}