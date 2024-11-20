using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json.Serialization;

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
    public DateTimeOffset? Timestamp { get; set; }

    public ChatMessage(MessageRole role, string content, DateTimeOffset? timestamp = null)
    {
        Role = role;
        Content = content;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
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

    public static ChatMessage FromMessage(Message message)
    {
        var role = Enum.Parse<MessageRole>(message.Role, true);
        return new ChatMessage(role, message.Content);
    }
}

public record Message(
    [property: JsonPropertyName("role")]
    string Role,
    [property: JsonPropertyName("content")]
    string Content
);
