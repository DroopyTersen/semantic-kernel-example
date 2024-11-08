using Microsoft.SemanticKernel;
using EnterpriseAI.Core.Models;
using EnterpriseAI.Core.Repositories;

namespace EnterpriseAI.Core.Services;

public class ConversationService
{
  private readonly IChatRepository _repository;
  private readonly AnswerGenerator _answerGenerator;

  public ConversationService(
      string conversationId,
      Kernel kernel,
      IChatRepository? repository = null)
  {
    ConversationId = string.IsNullOrEmpty(conversationId)
        ? throw new ArgumentException("Conversation ID cannot be null or empty", nameof(conversationId))
        : conversationId;
    _answerGenerator = new AnswerGenerator(kernel);
    _repository = repository ?? new InMemoryChatRepository();
  }

  public async Task<string> SubmitMessageAsync(string message)
  {
    // Create and save user message
    var userMessage = new ChatMessage(MessageRole.User, message);
    await _repository.SaveMessage(ConversationId, userMessage);
    var conversation = _repository.LoadConversation(ConversationId);

    var response = await _answerGenerator.AnswerQuestion(conversation) ?? "Error: Unable to respond";

    await _repository.SaveMessage(ConversationId, new ChatMessage(MessageRole.Assistant, response));

    return response;
  }

  public IReadOnlyList<ChatMessage> GetAllMessages() =>
      _repository.LoadConversation(ConversationId).ToList();

  public string ConversationId { get; }
}