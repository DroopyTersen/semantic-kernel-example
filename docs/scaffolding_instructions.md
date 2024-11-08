# EnterpriseAI Project Setup Documentation

This document outlines the complete setup process for the EnterpriseAI solution, including the initial project scaffolding and Semantic Kernel integration.

```
EnterpriseAI/
├── EnterpriseAI.sln
├── EnterpriseAI.API/
│   ├── Controllers/
│   └── Properties/
├── EnterpriseAI.Core/
│   ├── Configuration/
│   ├── Models/
│   ├── Plugins/
│   ├── Repositories/
│   └── Services/
└── EnterpriseAI.CLI/
```

## Initial Solution Setup

First, create the solution and project structure:

```bash
# Create solution and directory
mkdir EnterpriseAI
cd EnterpriseAI
dotnet new sln -n EnterpriseAI

# Create projects
dotnet new classlib -n EnterpriseAI.Core
dotnet new webapi -n EnterpriseAI.API
dotnet new console -n EnterpriseAI.CLI

# Add projects to solution
dotnet sln add EnterpriseAI.Core/EnterpriseAI.Core.csproj
dotnet sln add EnterpriseAI.API/EnterpriseAI.API.csproj
dotnet sln add EnterpriseAI.CLI/EnterpriseAI.CLI.csproj

# Add project references
cd EnterpriseAI.API
dotnet add reference ../EnterpriseAI.Core/EnterpriseAI.Core.csproj
cd ../EnterpriseAI.CLI
dotnet add reference ../EnterpriseAI.Core/EnterpriseAI.Core.csproj

# Add Swagger packages to API project
cd ../EnterpriseAI.API
dotnet add package Swashbuckle.AspNetCore

# Return to solution root
cd ..

# Restore and build
dotnet restore
dotnet build
```

## Project Structure

After running these commands, you'll have the following structure:

```
EnterpriseAI/
├── EnterpriseAI.sln
├── EnterpriseAI.Core/
├── EnterpriseAI.API/
└── EnterpriseAI.CLI/
```

- **EnterpriseAI.Core**: Class library containing shared business logic
- **EnterpriseAI.API**: Web API with Swagger integration
- **EnterpriseAI.CLI**: Console application for testing Core functionality

## Running the Projects

You can run the projects using these commands:

```bash
# Run the API
dotnet run --project EnterpriseAI.API

# Run the CLI
dotnet run --project EnterpriseAI.CLI

# Run API with watch mode (auto-recompile)
dotnet watch run --project EnterpriseAI.API
```

## VS Code Integration

Create `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "run-api",
      "command": "dotnet",
      "type": "process",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/EnterpriseAI.API/EnterpriseAI.API.csproj"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "run-cli",
      "command": "dotnet",
      "type": "process",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/EnterpriseAI.CLI/EnterpriseAI.CLI.csproj"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch-api",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "--project",
        "${workspaceFolder}/EnterpriseAI.API/EnterpriseAI.API.csproj"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

## Add Packages

Add required packages to the Core project:

```bash
# Add required packages to Core project
cd EnterpriseAI.Core
dotnet add package Microsoft.SemanticKernel
dotnet add package Azure.Search.Documents
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
dotnet add package Microsoft.Extensions.Configuration.Binder

# Add API packages
cd ../EnterpriseAI.API
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Swashbuckle.AspNetCore
```

### Configuration Files

#### AIServiceConfig.cs

```csharp
namespace EnterpriseAI.Core.Configuration;

public class AIServiceConfig
{
  public required string OpenAIApiKey { get; set; }
  public required string AzureOpenAIEndpoint { get; set; }
  public required string AzureOpenAIApiKey { get; set; }
  public required string ModelDeploymentName { get; set; }
  public required string EmbeddingDeploymentName { get; set; }
  public required string SearchServiceEndpoint { get; set; }
  public required string SearchServiceApiKey { get; set; }
  public required string SearchIndexName { get; set; }
}
```

#### KernelService.cs

```csharp
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

namespace EnterpriseAI.Core.Services;

public class KernelService
{
    private readonly Kernel _kernel;
    private readonly AIServiceConfig _config;

    public KernelService(IConfiguration configuration)
    {
        _config = configuration.GetSection("AIService").Get<AIServiceConfig>()
            ?? throw new InvalidOperationException("AIService configuration is missing");

        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: _config.ModelDeploymentName,
                endpoint: _config.AzureOpenAIEndpoint,
                apiKey: _config.AzureOpenAIKey
            );

        if (!string.IsNullOrEmpty(_config.OpenAIApiKey))
        {
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4-turbo-preview",
                apiKey: _config.OpenAIApiKey
            );
        }

        _kernel = builder.Build();
    }

    public Kernel Kernel => _kernel;
}
```

### Configuration Settings

#### appsettings.json (API & CLI)

```json
{
  ...
  "AIService": {
    "AzureOpenAIEndpoint": "https://oai-ai-demo-east.openai.azure.com/",
    "ModelDeploymentName": "gpt-4o-global",
    "EmbeddingDeploymentName": "text-embedding-3-large",
    "SearchServiceEndpoint": "https://srch-std-ai-demo.search.windows.net",
    "SearchIndexName": "idx-chunks"
  }
}
```

### Managing Secrets

Each project (API and CLI) needs its own local settings file for secrets.

1. For the API:

   ```bash
   cd EnterpriseAI.API
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

2. For the CLI:

   ```bash
   cd EnterpriseAI.CLI
   cp appsettings.local.template.json appsettings.local.json
   # Edit appsettings.local.json with your secrets
   ```

The `appsettings.local.json` files are gitignored and won't be committed.

### Program.cs Updates

#### API Program.cs

```csharp
using EnterpriseAI.Core.Configuration;
using EnterpriseAI.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services
builder.Services.Configure<AIServiceConfig>(
    builder.Configuration.GetSection("AIService"));
builder.Services.AddSingleton<KernelService>();

// ... rest of your Program.cs setup
```

#### CLI Program.cs

```csharp
using Microsoft.Extensions.Configuration;
using EnterpriseAI.Core.Configuration;
using EnterpriseAI.Core.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var kernelService = new KernelService(configuration);

// Your CLI logic here
```

## Verification

To verify the setup:

1. Build the solution:

```bash
dotnet build
```

2. Run the API:

```bash
dotnet run --project EnterpriseAI.API
```

3. Check Swagger UI at http://localhost:5000/swagger or https://localhost:5001/swagger

4. Run the CLI:

```bash
dotnet run --project EnterpriseAI.CLI
```

## Next Steps

1. Add your OpenAI and Azure OpenAI credentials using user secrets
2. Create your first Semantic Kernel plugin in the Core project
3. Implement API endpoints that utilize the Semantic Kernel service
4. Create CLI commands to test Semantic Kernel functionality
