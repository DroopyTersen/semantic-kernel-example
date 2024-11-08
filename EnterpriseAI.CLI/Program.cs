using Microsoft.Extensions.Configuration;
using EnterpriseAI.Core.Configuration;
using EnterpriseAI.Core.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "./EnterpriseAI.CLI"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var aiConfig = configuration.GetSection("AIService").Get<AIServiceConfig>()
    ?? throw new InvalidOperationException("AIService configuration is missing");

var kernelService = new KernelService(aiConfig);

var conversation = new ConversationService("123", kernel: kernelService.Kernel);
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

