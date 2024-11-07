using Microsoft.SemanticKernel;

namespace SegalAI.API.Models;

public record ChatMessage(
    string Role,
    string Content,
    DateTimeOffset Timestamp
)
{
  public static ChatMessage FromKernelMessage(ChatMessageContent message) => new(
      Role: message.Role.ToString(),
      Content: message.Content ?? "",
      Timestamp: DateTimeOffset.UtcNow
  );
}

public record ChatConversationResponse(
    string ConversationId,
    IReadOnlyList<ChatMessage> Messages
);

public record SubmitMessageRequest(
    string Message
);

public record SubmitMessageResponse(
    string Response
);