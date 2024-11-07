using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SegalAI.Core.Repositories;


namespace SegalAI.Core.Services;

public class RagService
{
  private readonly ChatHistory _history;
  private readonly Kernel _kernel;
  private readonly IChatRepository _repository;
  public string ConversationId { get; }

  public RagService(
      string conversationId,
      Kernel kernel,
      IChatRepository? repository = null)
  {
    ConversationId = conversationId;
    _kernel = kernel;
    _repository = repository ?? new InMemoryChatRepository(); ;
    _history = new ChatHistory();

    // Load existing messages from repository and convert to Semantic Kernel format
    var existingMessages = _repository.LoadConversation(conversationId);
    foreach (var message in existingMessages)
    {
      _history.Add(message);
    }
  }

  public async Task<string> SubmitMessage(string message)
  {
    // Create and save user message
    var userMessage = new ChatMessageContent(AuthorRole.User, message);
    await _repository.SaveMessage(ConversationId, userMessage);

    // Add to Semantic Kernel history
    _history.Add(userMessage);

    // Get chat completion service from the kernel
    var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

    // Get response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(_history);
    if (result != null)
    {
      await _repository.SaveMessage(ConversationId, result);
      _history.Add(result);
      return result.ToString();
    }
    return "Unable to get response from AI";
  }

  public IReadOnlyList<ChatMessageContent> GetAllMessages() =>
      _repository.LoadConversation(ConversationId).ToList();
}