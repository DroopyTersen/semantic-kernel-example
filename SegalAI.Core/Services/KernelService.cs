using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using SegalAI.Core.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SegalAI.Core.Services;

public class KernelService
{
  private readonly Kernel _kernel;
  private readonly AIServiceConfig _config;

  public KernelService(IConfiguration configuration)
  {
    _config = configuration.GetSection("AIService").Get<AIServiceConfig>()
        ?? throw new InvalidOperationException("AIService configuration is missing");
    Console.WriteLine(_config.AzureOpenAIApiKey);
    var builder = Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: _config.ModelDeploymentName,
            endpoint: _config.AzureOpenAIEndpoint,
            apiKey: _config.AzureOpenAIApiKey
        );

    _kernel = builder.Build();
  }

  public Kernel Kernel => _kernel;
  public IChatCompletionService ChatCompletionService => _kernel.GetRequiredService<IChatCompletionService>();
}