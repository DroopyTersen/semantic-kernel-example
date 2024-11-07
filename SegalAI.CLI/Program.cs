using Azure;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using SegalAI.Core.Configuration;
using SegalAI.Core.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "./SegalAI.CLI"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var aiConfig = configuration.GetSection("AIService").Get<AIServiceConfig>()
    ?? throw new InvalidOperationException("AIService configuration is missing");

Console.WriteLine("Config", configuration);

var kernelService = new KernelService(aiConfig);
var searchClient = new SearchClient(
        new Uri(aiConfig.SearchServiceEndpoint),
        aiConfig.SearchIndexName,
        new AzureKeyCredential(aiConfig.SearchServiceApiKey));
var searchService = new AzureSearchService(searchClient);
var conversation = new RagService("123", kernelService.Kernel, searchService: searchService);
// Initiate a back-and-forth chat
string? userInput;
do
{
  // Collect user input
  Console.Write("User > ");
  userInput = Console.ReadLine();

  // Add user input
  var result = await conversation.SubmitMessageAsync(userInput ?? "No input provied");

  // Print the results
  Console.WriteLine("Assistant > " + result);
} while (userInput is not null);

