using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace EnterpriseAI.Core.Services;

public class AzureSearchService(SearchClient searchClient) : IHybridSearchService
{
    private readonly string _ASSISTANT_ID = "37f7e52f-9656-4d00-87c7-b17ba486d525";
    public async Task<IEnumerable<SearchDocument>> QueryDocumentsAsync(
        SearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        if (criteria.Query is null && criteria.Embedding is null)
        {
            throw new ArgumentException("Either query or embedding must be provided");
        }

        //var documentContents = string.Empty;
        var top = criteria?.Top ?? 3;

        SearchOptions searchOptions = new SearchOptions
        {
            QueryType = SearchQueryType.Semantic,
            Select =
      {
        "id",
        "assistantId",
        "chunkType",
        "dataSourceId",
        "dataSourceTitle",
        "dataSourceType",
        "pageNumber",
        "position",
        "content",
      },
            Filter = $"assistantId eq '{_ASSISTANT_ID}' and chunkType eq 'text'",
            SemanticSearch = new()
            {
                SemanticConfigurationName = "my-semantic-config",
            },
            Size = top,
        };

        if (criteria?.Embedding != null)
        {
            var vectorQuery = new VectorizedQuery(criteria.Embedding)
            {
                // if semantic ranker is enabled, we need to set the rank to a large number to get more
                // candidates for semantic reranking
                KNearestNeighborsCount = 50
            };
            vectorQuery.Fields.Add("embedding");
            searchOptions.VectorSearch = new();
            searchOptions.VectorSearch.Queries.Add(vectorQuery);
        }

        var searchResultResponse = await searchClient.SearchAsync<SearchDocument>(
            criteria?.Query ?? "*", searchOptions, cancellationToken);
        if (searchResultResponse.Value is null)
        {
            throw new InvalidOperationException("fail to get search result");
        }

        SearchResults<SearchDocument> searchResult = searchResultResponse.Value;
        return searchResult.GetResults().Select(r => r.Document).ToList();
    }
}