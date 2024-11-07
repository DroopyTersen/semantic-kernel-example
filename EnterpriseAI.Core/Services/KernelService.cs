#pragma warning disable SKEXP0010

using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using EnterpriseAI.Core.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;

namespace EnterpriseAI.Core.Services;

public class KernelService
{
  private readonly Kernel _kernel;
  private readonly AIServiceConfig config;

  public KernelService(AIServiceConfig config)
  {
    this.config = config ?? throw new ArgumentNullException(nameof(config));

    ValidateConfiguration(config);

    var builder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: config.ModelDeploymentName,
            endpoint: config.AzureOpenAIEndpoint,
            apiKey: config.AzureOpenAIApiKey
        );
    builder.AddAzureOpenAITextEmbeddingGeneration(
        deploymentName: config.EmbeddingDeploymentName,
        endpoint: config.AzureOpenAIEndpoint,
        apiKey: config.AzureOpenAIApiKey,
        dimensions: 1024
    );

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