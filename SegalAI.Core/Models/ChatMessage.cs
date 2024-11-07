using System;

namespace SegalAI.Core.Models;

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
}