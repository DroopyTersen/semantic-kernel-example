namespace SegalAI.Core.Services;
using System.Text.Json.Serialization;

public interface IHybridSearchService
{
  Task<IEnumerable<SearchDocument>> QueryDocumentsAsync(
      SearchCriteria criteria,
      CancellationToken cancellationToken = default);
}

public record SearchCriteria
{
  public string? Query { get; set; }
  public float[]? Embedding { get; set; }
  public int? Top { get; init; }
}


public record SearchDocument
{
  [JsonPropertyName("id")]
  public required string Id { get; init; }

  [JsonPropertyName("assistantId")]
  public required string AssistantId { get; init; }

  [JsonPropertyName("dataSourceId")]
  public required string DataSourceId { get; init; }

  [JsonPropertyName("dataSourceTitle")]
  public required string DataSourceTitle { get; init; }

  [JsonPropertyName("dataSourceType")]
  public required string DataSourceType { get; init; }

  [JsonPropertyName("dataSourceAuthor")]
  public string? DataSourceAuthor { get; init; }

  [JsonPropertyName("position")]
  public required int Position { get; init; }

  [JsonPropertyName("content")]
  public required string Content { get; init; }

  [JsonPropertyName("embedding")]
  public float[]? Embedding { get; init; }

  [JsonPropertyName("createdAt")]
  public DateTimeOffset? CreatedAt { get; init; }

  [JsonPropertyName("data")]
  public string? Data { get; init; }

  [JsonPropertyName("pageNumber")]
  public required int PageNumber { get; init; }

  [JsonPropertyName("chunkType")]
  public required string ChunkType { get; init; }
}
