using Microsoft.SemanticKernel.ChatCompletion;
using EnterpriseAI.Core.Models;

public class QuestionExtractor
{
  private static string GetSystemPrompt() => $@"
Convert a given chat transcript into a standalone query designed for retrieving relevant external context in a retrieval-augmented generation (RAG) system.

The output query should capture the core information needs of the user without any references to the original chat structure itself, making it clear, direct, and independent from preceding context.

- Extract the primary question or information need from the chat transcript.
- Use the context of the conversation to determine the most actionable part of the user's request.
- Be precise in choosing details that help retrieve relevant information, such as product names, important figures, names, or other key phrases mentioned in the transcript.

[Today's Date: {DateTimeOffset.UtcNow:yyyy-MM-dd}]

# Steps:

1. **Review Chat Transcript**: Read the provided chat transcript to understand what information is being sought.
2. **Identify Key Question or Information Need**: Identify the main question or statement that represents the user's information needs.
3. **Create Standalone Query**: Rephrase and transform the user's core information request into a single, comprehensive, and independent query.

# Output Format

The output should be a single, clear, and actionable query in plain text.

# Examples

**Example 1:**

**Input Chat Transcript:**
User: Hey, what do you know about Tesla's financial performance lately?
Assistant: Is there anything specific about Tesla you'd like to know? For example, quarterly results or stock performance?
User: Just the recent quarterly earnings.

**Output Query:**
""Tesla recent quarterly earnings report""

**Example 2:**

**Input Chat Transcript:**
User: I'm trying to learn about that new regulation in Europe related to data privacy. Something that affects tech companies a lot.
Assistant: Are you referring to GDPR or some other recent law?
User: No, the more recent one involving online data sharing policies.

**Output Query:**
""most recent European regulation on data privacy affecting online data sharing policies""

# Notes

- Ensure that the final query is standalone and that it retains full clarity without needing the preceding conversation context.
- Avoid including conversational instructions like ""tell me about"". Instead, directly state the need as a complete request.
- Focus on extracting specific details, such as dates or names where possible, to make the query more precise and effective in retrieving the correct information.";

  private readonly IChatCompletionService _chatService;

  public QuestionExtractor(IChatCompletionService chatService)
  {
    _chatService = chatService;
  }

  public async Task<string> ExtractQuestion(IEnumerable<ChatMessage> messages)
  {
    var transcript = SerializeTranscript(messages);
    var history = CreateChatHistory(transcript);

    var result = await _chatService.GetChatMessageContentAsync(history);
    return result?.Content ?? string.Empty;
  }

  private static string SerializeTranscript(IEnumerable<ChatMessage> messages) =>
      string.Join("\n\n", messages.Select(m => $"{m.Role}: {m.Content}"));

  private static ChatHistory CreateChatHistory(string transcript)
  {
    var history = new ChatHistory();
    history.AddSystemMessage(GetSystemPrompt());
    history.AddUserMessage($"Please extract a standalone question from this chat transcript:\n\n{transcript}");
    return history;
  }
}