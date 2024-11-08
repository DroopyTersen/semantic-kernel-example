#pragma warning disable SKEXP0010

using Microsoft.SemanticKernel;
using EnterpriseAI.Core.Configuration;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.Extensions.DependencyInjection;
using Azure;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using EnterpriseAI.Core.Plugins;
using Azure.Search.Documents;

namespace EnterpriseAI.Core.Services;

public class KernelService
{
  private readonly Kernel _kernel;
  private readonly AIServiceConfig config;

  public KernelService(AIServiceConfig config)
  {
    this.config = config ?? throw new ArgumentNullException(nameof(config));

    ValidateConfiguration(config);

    var builder = Kernel.CreateBuilder();

    // Add Azure OpenAI chat completion
    builder.AddAzureOpenAIChatCompletion(
        deploymentName: config.ModelDeploymentName,
        endpoint: config.AzureOpenAIEndpoint,
        apiKey: config.AzureOpenAIApiKey
    );


    // Create and register the embedding service directly
    builder.Services.AddSingleton<ITextEmbeddingGenerationService>((_) =>
    {
      return new AzureOpenAITextEmbeddingGenerationService(
          deploymentName: config.EmbeddingDeploymentName,
          endpoint: config.AzureOpenAIEndpoint,
          apiKey: config.AzureOpenAIApiKey,
          dimensions: 1024
      );
    });

    // Add Azure Search service
    builder.Services.AddSingleton<IHybridSearchService>(sp =>
    {
      var searchClient = new SearchClient(
      new Uri(config.SearchServiceEndpoint),
      config.SearchIndexName,
      new AzureKeyCredential(config.SearchServiceApiKey));
      return new AzureSearchService(searchClient);
    });

    // Add the "Search" plugin which will allow tool use
    builder.Plugins.AddFromType<AzureAISearchPlugin>("Search");

    _kernel = builder.Build();
  }

  public Kernel Kernel => _kernel;

  private void ValidateConfiguration(AIServiceConfig config)
  {
    if (string.IsNullOrEmpty(config.ModelDeploymentName))
      throw new ArgumentException("ModelDeploymentName cannot be null or empty", nameof(config));
    if (string.IsNullOrEmpty(config.AzureOpenAIEndpoint))
      throw new ArgumentException("AzureOpenAIEndpoint cannot be null or empty", nameof(config));
    if (string.IsNullOrEmpty(config.AzureOpenAIApiKey))
      throw new ArgumentException("AzureOpenAIApiKey cannot be null or empty", nameof(config));
    if (string.IsNullOrEmpty(config.EmbeddingDeploymentName))
      throw new ArgumentException("EmbeddingDeploymentName cannot be null or empty", nameof(config));
  }
}