using Microsoft.Extensions.Configuration;
using SegalAI.Core.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "./SegalAI.CLI"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Console.WriteLine("Config", configuration);

var kernelService = new KernelService(configuration);
var conversation = new RagService("123", kernelService.Kernel);
// Initiate a back-and-forth chat
string? userInput;
do
{
  // Collect user input
  Console.Write("User > ");
  userInput = Console.ReadLine();

  // Add user input
  var result = await conversation.SubmitMessage(userInput ?? "No input provied");

  // Print the results
  Console.WriteLine("Assistant > " + result);
} while (userInput is not null);

