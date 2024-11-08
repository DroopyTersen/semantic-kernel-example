using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace EnterpriseAI.Core.Models;

public enum MessageRole
{
    System,
    User,
    Assistant
}

public class ChatMessage
{
    public MessageRole Role { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public ChatMessage(MessageRole role, string content)
    {
        Role = role;
        Content = content;
        Timestamp = DateTimeOffset.UtcNow;
    }
    public static ChatMessageContent ToKernelMessage(ChatMessage message)
    {
        var role = message.Role switch
        {
            MessageRole.System => AuthorRole.System,
            MessageRole.User => AuthorRole.User,
            MessageRole.Assistant => AuthorRole.Assistant,
            _ => throw new ArgumentException($"Unknown role: {message.Role}")
        };
        return new ChatMessageContent(role, message.Content);
    }
}