namespace EnterpriseAI.Core.Services;

using System.Text.Json.Serialization;

/// <summary>
/// Provides hybrid search capabilities combining semantic and keyword-based search approaches.
/// </summary>
/// <remarks>
/// Hybrid search combines two search methodologies:
/// 1. Semantic Search: Using vector embeddings to find semantically similar content
/// 2. Keyword Search: Traditional text-based search for exact or fuzzy matches
///
/// The search can be performed using either or both approaches based on the provided criteria.
/// </remarks>
public interface IHybridSearchService
{
    /// <summary>
    /// Queries documents using a hybrid search approach.
    /// </summary>
    /// <param name="criteria">Search parameters including text query and/or vector embedding</param>
    /// <param name="cancellationToken">Optional token to cancel the operation</param>
    /// <returns>A collection of search documents ordered by relevance</returns>
    /// <remarks>
    /// - If only Query is provided, performs keyword-based search
    /// - If only Embedding is provided, performs semantic similarity search
    /// - If both are provided, combines results using a hybrid approach
    /// </remarks>
    Task<IEnumerable<SearchDocument>> QueryDocumentsAsync(
        SearchCriteria criteria,
        CancellationToken cancellationToken = default
    );
    // TODO: Method to delete all chunks for a data source
    // TODO: Method to get all chunks for a data source
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
