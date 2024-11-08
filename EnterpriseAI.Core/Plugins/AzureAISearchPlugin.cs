using System.ComponentModel;
using System.Text.Json.Serialization;
using EnterpriseAI.Core.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace EnterpriseAI.Core.Plugins;

public class AzureAISearchPlugin
{
  private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
  private readonly IHybridSearchService _searchService;

  public AzureAISearchPlugin(ITextEmbeddingGenerationService textEmbeddingGenerationService, IHybridSearchService searchService)
  {
    _textEmbeddingGenerationService = textEmbeddingGenerationService;
    _searchService = searchService;
  }

  [KernelFunction("search_relevant_context")]
  [Description("Search for relevant context to help answer the user's question with Company data. You should almost ALWAYS use this tool before answering a user's question.")]
  [return: Description("Returns a list of relevent chunks of texts from various data sources.")]
  public async Task<string> SearchAsync([Description("A standalone query designed for retrieving relevant external context in a retrieval-augmented generation (RAG) system.\n\nThe output query should capture the core information needs of the user without any references to the original chat structure itself, making it clear, direct, and independent from preceding context.")] string standaloneQuery)
  {
    // Convert string query to vector
    Console.WriteLine("Embedding: " + standaloneQuery);
    var embedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(standaloneQuery);
    var searchCriteria = new SearchCriteria
    {
      Query = standaloneQuery,
      Embedding = embedding.ToArray(),
      Top = 10
    };
    var results = await _searchService.QueryDocumentsAsync(searchCriteria);
    var context = results.Any()
    ? string.Join("\n\n", results.Select(d => $"Title: {d.DataSourceTitle}\n{d.Content}"))
    : string.Empty;

    return context;
  }



  private sealed class IndexSchema
  {

    [JsonPropertyName("chunk")]
    public string? Chunk { get; set; }

    [JsonPropertyName("text-vector")]
    public ReadOnlyMemory<float> Vector { get; set; }
  }
}
