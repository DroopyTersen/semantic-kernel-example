using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SegalAI.Core.Models;

public static class ChatMessageExtensions
{
  public static ChatMessageContent ToKernelMessage(this ChatMessage message)
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

  public static ChatMessage ToDomainMessage(this ChatMessageContent message)
  {
    var role = message.Role == AuthorRole.System ? MessageRole.System :
              message.Role == AuthorRole.User ? MessageRole.User :
              message.Role == AuthorRole.Assistant ? MessageRole.Assistant :
              throw new ArgumentException($"Unknown role: {message.Role}");

    return new ChatMessage(role, message.Content ?? string.Empty);
  }
}