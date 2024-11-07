namespace SegalAI.Core.Configuration;

public class AIServiceConfig
{
  public required string OpenAIApiKey { get; set; }
  public required string AzureOpenAIEndpoint { get; set; }
  public required string AzureOpenAIApiKey { get; set; }
  public required string ModelDeploymentName { get; set; }
  public required string EmbeddingDeploymentName { get; set; }
}