using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SegalAI.Core.Models;
using SegalAI.Core.Repositories;
using Microsoft.SemanticKernel.Embeddings;

namespace SegalAI.Core.Services;

public class RagService
{
  private readonly Kernel _kernel;
  private readonly IChatRepository _repository;
  private readonly IHybridSearchService _searchService;
  private readonly QuestionExtractor _questionExtractor;
  private readonly AnswerGenerator _answerGenerator;

  public RagService(
      string conversationId,
      Kernel kernel,
      IChatRepository? repository = null,
      IHybridSearchService? searchService = null)
  {
    ConversationId = string.IsNullOrEmpty(conversationId)
        ? throw new ArgumentException("Conversation ID cannot be null or empty", nameof(conversationId))
        : conversationId;
    _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    _repository = repository ?? new InMemoryChatRepository();
    _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));

    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    _questionExtractor = new QuestionExtractor(chatService);
    _answerGenerator = new AnswerGenerator(chatService);
  }

  private async Task<IEnumerable<SearchDocument>> RetrieveRelevantChunksAsync(string question)
  {
    var embeddingService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
    var embedding = await embeddingService.GenerateEmbeddingAsync(question);

    var searchCriteria = new SearchCriteria
    {
      Query = question, // Enable semantic search
      Embedding = embedding.ToArray(), // Enable vector search
      Top = 10 // Limit results
    };

    return await _searchService.QueryDocumentsAsync(searchCriteria);
  }

  public async Task<string> SubmitMessageAsync(string message)
  {
    // Create and save user message
    var userMessage = new ChatMessage(MessageRole.User, message);
    await _repository.SaveMessage(ConversationId, userMessage);
    var conversation = _repository.LoadConversation(ConversationId);

    // Extract standalone question and retrieve relevant documents
    var question = await _questionExtractor.ExtractQuestion(conversation);
    Console.WriteLine($"Standalone Question: {question}");
    var documents = await RetrieveRelevantChunksAsync(question);
    // Console.WriteLine($"Relevant Chunks:\n{string.Join("\n\n-----------\n\n", documents.Select(d => d.Content))}");

    // Prepare context from retrieved documents
    var context = documents.Any()
        ? string.Join("\n\n", documents.Select(d => $"Title: {d.DataSourceTitle}\n{d.Content}"))
        : string.Empty;

    // Get response using AnswerGenerator
    var response = await _answerGenerator.AnswerQuestion(context, conversation) ?? "Error: Unable to respond";

    await _repository.SaveMessage(ConversationId, new ChatMessage(MessageRole.Assistant, response));

    return response;
  }

  public IReadOnlyList<ChatMessage> GetAllMessages() =>
      _repository.LoadConversation(ConversationId).ToList();

  public string ConversationId { get; }
}